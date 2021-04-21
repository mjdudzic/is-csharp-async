using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex3
{
	public class ParallelForLoopTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public ParallelForLoopTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void LoopManyOperationsInParallel()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			Parallel.For(0, 8, (i, state) =>
			{
				RunComplexCalculation(i);
			});

			stopWatch.Stop();

			_testOutputHelper.WriteLine($"Operation completed at {stopWatch.ElapsedMilliseconds} ms");
		}

		private void RunComplexCalculation(int executionIndex)
		{
			_testOutputHelper.WriteLine(
				$"Starting parallel operation {executionIndex} - Thread {Thread.CurrentThread.ManagedThreadId}");
			Thread.Sleep(100);
		}
	}
}
