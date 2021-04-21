using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex3
{
	public class ParallelInvokeTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public ParallelInvokeTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void InvokeManyOperationsInParallel()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			Parallel.Invoke(
				RunComplexCalculation,
				RunComplexCalculation,
				RunComplexCalculation);

			stopWatch.Stop();

			_testOutputHelper.WriteLine($"Operation completed at {stopWatch.ElapsedMilliseconds} ms");

			stopWatch.Restart();
			var tasks = new[]
			{
				Task.Run(RunComplexCalculation),
				Task.Run(RunComplexCalculation),
				Task.Run(RunComplexCalculation)
			};

			Task.WaitAll(tasks);

			stopWatch.Stop();

			_testOutputHelper.WriteLine($"Operation completed at {stopWatch.ElapsedMilliseconds} ms");
		}

		private void RunComplexCalculation()
		{
			_testOutputHelper.WriteLine(
				$"Starting parallel operation - Thread {Thread.CurrentThread.ManagedThreadId}");
			Thread.Sleep(100);
		}
	}
}
