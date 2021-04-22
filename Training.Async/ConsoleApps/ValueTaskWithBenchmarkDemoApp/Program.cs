using System;
using BenchmarkDotNet.Running;

namespace ValueTaskWithBenchmarkDemoApp
{
	class Program
	{
		static void Main(string[] args)
		{
			BenchmarkRunner.Run<ServiceBenchmark>();
		}
	}
}
