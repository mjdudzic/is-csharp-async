using System;
using System.Collections.Generic;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;

namespace WebAppDotNet5.Infrastructure.Tracing.Jaeger.Helpers
{
	public class TracingHelper : ITracingHelper
	{
		public string TracingKeysPropertyName => "TracingKeys";
		public string ContextTraceIdPropertyName => "ContextTraceId";
		public string MessageOperationActionName => "message";
		public string JobOperationActionName => "job";
		public string ExternalApiActionName => "ext-api";
		public string MessageSubscriberOperationActionName => "messageSubscriber";

		public ITracer Tracer { get; }

		public TracingHelper(ITracer tracer) 
			=> Tracer = tracer;

		public IScope StartServerSpan(IDictionary<string, string> headers, string operationName)
		{
			ISpanBuilder spanBuilder;

			try
			{
				var parentSpanCtx = Tracer.Extract(BuiltinFormats.TextMap, new TextMapExtractAdapter(headers));

				spanBuilder = Tracer.BuildSpan(operationName);
				if (parentSpanCtx != null)
				{
					spanBuilder = spanBuilder.AsChildOf(parentSpanCtx);
				}
			}
			catch (Exception)
			{
				spanBuilder = Tracer.BuildSpan(operationName);
			}

			return spanBuilder.WithTag(Tags.SpanKind, Tags.SpanKindConsumer).StartActive(true);
		}

		public IDictionary<string, string> GetActiveSpanTracingKeys() 
			=> GetActiveSpanTracingKeys(BuiltinFormats.TextMap);

		public IDictionary<string, string> GetActiveSpanTracingKeys(IFormat<ITextMap> format)
		{
			if (Tracer.ActiveSpan == null)
			{
				return new Dictionary<string, string>();
			}

			var dictionary = new Dictionary<string, string>();
			Tracer.Inject(Tracer.ActiveSpan.Context, format, new TextMapInjectAdapter(dictionary));

			return dictionary;
		}

		public void LogToSpan(string key, object obj)
		{
			Tracer.ActiveSpan?.Log(new Dictionary<string, object>
			{
				{key, obj}
			});
		}

		public void LogToSpan(Dictionary<string, object> logsDictionary) 
			=> Tracer.ActiveSpan?.Log(logsDictionary);

		public void LogSpanException(Exception exception)
		{
			if (Tracer.ActiveSpan == null)
				return;

			Tracer.ActiveSpan.Log(new Dictionary<string, object> { { "exception", exception } });
			Tags.Error.Set(Tracer.ActiveSpan, true);
		}
	}
}