using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;

namespace WebAppDotNet5.Infrastructure.Tracing.Jaeger.Middlewares
{
	public class TracingMiddleware
	{
		private readonly RequestDelegate _next;

		public TracingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, ITracer tracer)
		{
			context.Request.EnableBuffering();

			if (IsRequestHttpMethodAllowed(context.Request) == false ||
				IsRequestPathAllowed(context.Request) == false)
			{
				await _next(context);

				return;
			}

			var extractedSpanContext = tracer.Extract(
				BuiltinFormats.HttpHeaders,
				new TextMapExtractAdapter(context.Request.Headers.ToDictionary(i => i.Key, i => i.Value.ToString())));

			using (var scope = tracer
				.BuildSpan($"{context.Request.Method} {context.Request.Path}")
				.AsChildOf(extractedSpanContext)
				.StartActive(true))
			{
				try
				{
					scope.Span.SetTag("ContextTenantIpAddress", GetTenantIpAddress(context));

					await LogRequest(context.Request, scope.Span);

					var originalBodyStream = context.Response.Body;

					using (var responseBody = new MemoryStream())
					{
						context.Response.Body = responseBody;

						await _next(context);

						await LogResponse(context.Response, scope.Span);

						await responseBody.CopyToAsync(originalBodyStream);
					}
				}
				catch (Exception e)
				{
					scope.Span.Log(new Dictionary<string, object> { { "exception", e } });
					Tags.Error.Set(scope.Span, true);
					throw;
				}

			}
		}

		private static string GetTenantIpAddress(HttpContext httpContext)
		{
			return httpContext
				?.Connection
				?.RemoteIpAddress.ToString();
		}

		private static async Task LogRequest(HttpRequest request, ISpan span)
		{
			if (request.ContentLength > 60000)
			{
				span.Log(new Dictionary<string, object> { { "requestBody", "[not included due to size limit]" } });
				return;
			}

			request.EnableBuffering();

			var buffer = new byte[Convert.ToInt32(request.ContentLength)];

			await request.Body.ReadAsync(buffer, 0, buffer.Length);

			var bodyAsText = Encoding.UTF8.GetString(buffer);

			request.Body.Position = 0;

			span.Log(new Dictionary<string, object> { { "requestBody", bodyAsText } });
		}

		private static async Task LogResponse(HttpResponse response, ISpan span)
		{
			response.Body.Seek(0, SeekOrigin.Begin);

			var responseText = await new StreamReader(response.Body).ReadToEndAsync();

			response.Body.Seek(0, SeekOrigin.Begin);

			span.Log(new Dictionary<string, object>
			{
				{ "responseCode", response.StatusCode },
				{ "responseBody", responseText }
			});
		}

		private static bool IsRequestHttpMethodAllowed(HttpRequest request)
		{
			var requestMethodsToTrace = new[] { HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete, HttpMethods.Patch };

			return requestMethodsToTrace.Contains(request.Method);
		}

		private static bool IsRequestPathAllowed(HttpRequest request)
		{
			return
				request.Path.Value.Contains("/dashboard", StringComparison.InvariantCultureIgnoreCase) == false &&
				request.Path.Value.Contains("/swagger", StringComparison.InvariantCultureIgnoreCase) == false;
		}
	}
}
