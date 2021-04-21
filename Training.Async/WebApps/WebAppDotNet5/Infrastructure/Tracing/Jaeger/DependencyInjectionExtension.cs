using System;
using System.Collections.Generic;
using System.Reflection;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;
using OpenTracing.Util;
using WebAppDotNet5.Infrastructure.Tracing.Jaeger.Configuration;
using WebAppDotNet5.Infrastructure.Tracing.Jaeger.Helpers;
using WebAppDotNet5.Infrastructure.Tracing.Jaeger.Middlewares;

namespace WebAppDotNet5.Infrastructure.Tracing.Jaeger
{
	internal static class DependencyInjectionExtension
	{
		public static IServiceCollection AddJaeger(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddTransient<ITracingHelper, TracingHelper>();

			var options = GetJaegerOptions(services);

			if (!options.Enabled)
			{
				var defaultTracer = CreateDefaultTracer();
				services.AddSingleton(defaultTracer);
				return services;
			}

			services.AddSingleton<ITracer>(sp =>
			{
				var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

				var reporter = new RemoteReporter.Builder()
					.WithSender(new UdpSender(options.UdpHost, options.UdpPort, options.MaxPacketSize))
					.WithLoggerFactory(loggerFactory)
					.Build();

				var sampler = GetSampler(options);

				var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;

				var tracer = new Tracer.Builder(serviceName)
					.WithReporter(reporter)
					.WithSampler(sampler)
					.Build();

				GlobalTracer.Register(tracer);

				return tracer;
			});

			return services;
		}

		public static IApplicationBuilder UseJaeger(this IApplicationBuilder app)
		{
			app.UseMiddleware<TracingMiddleware>();

			return app;
		}

		public static IScope StartServerSpan(ITracer tracer, IDictionary<string, string> headers, string operationName)
		{
			ISpanBuilder spanBuilder;
			try
			{
				var parentSpanCtx = tracer.Extract(BuiltinFormats.TextMap, new TextMapExtractAdapter(headers));

				spanBuilder = tracer.BuildSpan(operationName);
				if (parentSpanCtx != null)
				{
					spanBuilder = spanBuilder.AsChildOf(parentSpanCtx);
				}
			}
			catch (Exception)
			{
				spanBuilder = tracer.BuildSpan(operationName);
			}

			return spanBuilder.WithTag(Tags.SpanKind, Tags.SpanKindConsumer).StartActive(true);
		}

		private static JaegerOptions GetJaegerOptions(IServiceCollection services)
		{
			using (var serviceProvider = services.BuildServiceProvider())
			{
				var configuration = serviceProvider.GetService<IConfiguration>();
				var options = new JaegerOptions();
				configuration.Bind("Jaeger", options);

				return options;
			}
		}

		private static ISampler GetSampler(JaegerOptions options)
		{
			return options.Sampler switch
			{
				"const" => (ISampler) new ConstSampler(true),
				"rate" => new RateLimitingSampler(options.MaxTracesPerSecond),
				"probabilistic" => new ProbabilisticSampler(options.SamplingRate),
				_ => new ConstSampler(true)
			};
		}

		public static ITracer CreateDefaultTracer()
			=> new Tracer.Builder(Assembly.GetEntryAssembly()?.FullName)
				.WithReporter(new NoopReporter())
				.WithSampler(new ConstSampler(false))
				.Build();
	}
}