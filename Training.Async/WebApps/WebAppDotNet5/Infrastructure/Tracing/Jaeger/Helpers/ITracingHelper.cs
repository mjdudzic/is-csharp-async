using System;
using System.Collections.Generic;
using OpenTracing;

namespace WebAppDotNet5.Infrastructure.Tracing.Jaeger.Helpers
{
	public interface ITracingHelper
	{
		string TracingKeysPropertyName { get; }
		string ContextTraceIdPropertyName { get; }
		string MessageOperationActionName { get; }
		string JobOperationActionName { get; }
		string ExternalApiActionName { get; }
		string MessageSubscriberOperationActionName { get; }
		ITracer Tracer { get; }
		IScope StartServerSpan(IDictionary<string, string> headers, string operationName);
		IDictionary<string, string> GetActiveSpanTracingKeys();
		void LogToSpan(string key, object obj);
		void LogToSpan(Dictionary<string, object> logsDictionary);
		void LogSpanException(Exception exception);
	}
}
