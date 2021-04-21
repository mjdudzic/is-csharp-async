using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex2
{
	public class TaskCancellationTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public TaskCancellationTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task StopTasksOnCancelRequestTest()
		{
			var cts = new CancellationTokenSource();
			var token = cts.Token;

			//cts.Cancel();

			var task1 = Task.Run(() =>
			{
				_testOutputHelper.WriteLine(
					$"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				while (token.IsCancellationRequested == false)
				{
					_testOutputHelper.WriteLine("Doing something ...");
					Thread.Sleep(1000);
				}
				_testOutputHelper.WriteLine("Cancelled!");

				//while (true)
				//{
				//	_testOutputHelper.WriteLine("Doing something ...");
				//	Thread.Sleep(1000);

				//	token.ThrowIfCancellationRequested();
				//}
			}, token);

			var task2 = task1.ContinueWith(parentTask =>
			{
				_testOutputHelper.WriteLine(
					$"Starting child task {Task.CurrentId} for parent {parentTask.Id} in thread {Thread.CurrentThread.ManagedThreadId}");
			}, token);

			var task3 = Task.Run(() =>
			{
				_testOutputHelper.WriteLine(
					$"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				Thread.Sleep(2000);
			}, token);

			await Task.Delay(1000);

			cts.Cancel();

			try
			{
				await Task.WhenAll(task1, task2, task3);
			}
			catch (Exception e)
			{
				_testOutputHelper.WriteLine(e.Message);
			}

			task2.IsCanceled.Should().BeTrue();
		}
	}
}
