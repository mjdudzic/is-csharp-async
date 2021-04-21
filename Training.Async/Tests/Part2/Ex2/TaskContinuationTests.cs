using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex2
{
	public class TaskContinuationTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public TaskContinuationTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task TasksChainAsParentAndChildTest()
		{
			_testOutputHelper.WriteLine($"Starting tasks - Thread {Thread.CurrentThread.ManagedThreadId}");

			var task1 = Task.Factory.StartNew(() =>
			{
				_testOutputHelper.WriteLine(
					$"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				Task.Factory.StartNew(() =>
				{
					Thread.Sleep(2000);
					_testOutputHelper.WriteLine(
						$"Starting child task1 {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
				}, TaskCreationOptions.AttachedToParent);

				Task.Factory.StartNew(() =>
				{
					Thread.Sleep(1000);
					_testOutputHelper.WriteLine(
						$"Starting child task2 {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
				});
			});

			await task1;

			_testOutputHelper.WriteLine($"Ending tasks - Thread {Thread.CurrentThread.ManagedThreadId}");

			task1.IsCompleted.Should().BeTrue();
		}

		[Fact]
		public async Task TasksChainWithContinuationsTest()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var task1 = Task.Run(() =>
			{
				_testOutputHelper.WriteLine(
					$"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				//throw new Exception("test");
			}).ContinueWith(parentTask =>
			{
				_testOutputHelper.WriteLine($"Parent task status is {parentTask.Status}");
				_testOutputHelper.WriteLine(
					$"Starting child task {Task.CurrentId} for parent {parentTask.Id} in thread {Thread.CurrentThread.ManagedThreadId}");
			}).ContinueWith(parentTask =>
			{
				_testOutputHelper.WriteLine(
					$"Starting child task {Task.CurrentId} for parent {parentTask.Id} in thread {Thread.CurrentThread.ManagedThreadId}");
			});

			_testOutputHelper.WriteLine($"Returned task ID is {task1.Id}");

			await task1;

			task1.IsCompleted.Should().BeTrue();
		}

		[Fact]
		public async Task TasksChainWithConditionalContinuationsTest()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			try
			{
				var task1 = Task.Run(() =>
				{
					_testOutputHelper.WriteLine(
						$"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

					throw new Exception("test");
				});

				var taskOnCompletion = task1.ContinueWith(parentTask =>
					{
						_testOutputHelper.WriteLine(
							$"Starting {TaskContinuationOptions.OnlyOnRanToCompletion} child task {Task.CurrentId} for parent {parentTask.Id} in thread {Thread.CurrentThread.ManagedThreadId}");
					}, TaskContinuationOptions.OnlyOnRanToCompletion);
				
				var taskOnFaulted = task1.ContinueWith(parentTask =>
					{
						_testOutputHelper.WriteLine(
							$"Starting {TaskContinuationOptions.OnlyOnFaulted} child task {Task.CurrentId} for parent {parentTask.Id} in thread {Thread.CurrentThread.ManagedThreadId}");
					}, TaskContinuationOptions.OnlyOnFaulted);

				await Task.WhenAny(taskOnCompletion, taskOnFaulted);

				task1.IsCompleted.Should().BeTrue();
			}
			catch (Exception e)
			{
				_testOutputHelper.WriteLine(e.Message);
			}
			
		}
	}
}
