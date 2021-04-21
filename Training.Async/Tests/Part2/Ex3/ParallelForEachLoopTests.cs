using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex3
{
	public class ParallelForEachLoopTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public ParallelForEachLoopTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void LoopManyOperationsInParallel()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var idList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
			var result = Parallel.ForEach(idList, (id, state) =>
			{
				RunComplexCalculation(id);
			});

			stopWatch.Stop();

			_testOutputHelper.WriteLine($"Operation completed at {stopWatch.ElapsedMilliseconds} ms");

			result.IsCompleted.Should().BeTrue();
		}

		[Fact]
		public void LoopManyOperationsInParallelWithBreakAndStop()
		{
			var cts = new CancellationTokenSource();
			var token = cts.Token;

			var idList = new List<int>();
			idList.AddRange(Enumerable.Range(1, 100));

			Task.Run(() =>
			{
				Thread.Sleep(3000);

				_testOutputHelper.WriteLine("Cancelling parallel operations");
				cts.Cancel();
			}, cts.Token);

			var result = Parallel.ForEach(
				idList,
				new ParallelOptions { MaxDegreeOfParallelism = 3 },
				(id, state) =>
				{
					_testOutputHelper.WriteLine($"Start processing data for id {id}");
					
					if (token.IsCancellationRequested)
					{
						_testOutputHelper.WriteLine("Stopping parallel processing");
						state.Break();
						//state.Stop();

						return;
					}

					//if (state.IsStopped)
					//{
					//	_testOutputHelper.WriteLine($"Parallel processing stopped so do not process data for id {id}");

					//	return;
					//}

					RunComplexCalculation(id);

					//if (id > 10)
					//{
					//	throw new Exception("test");
					//}

					_testOutputHelper.WriteLine($"End processing data for id {id}");
				});

			_testOutputHelper.WriteLine($"Parallel processing stopped at {result.LowestBreakIteration} iteration");

			result.IsCompleted.Should().BeFalse();
		}

		private void RunComplexCalculation(int id)
		{
			_testOutputHelper.WriteLine(
				$"Starting parallel operation {id} - Thread {Thread.CurrentThread.ManagedThreadId}");
			Thread.Sleep(1000);
		}
	}
}
