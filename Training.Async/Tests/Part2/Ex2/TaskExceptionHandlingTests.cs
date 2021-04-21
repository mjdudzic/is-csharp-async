using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex2
{
	public class TaskExceptionHandlingTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public TaskExceptionHandlingTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void NotHandledTaskExceptionTest()
		{
			var exceptionHandled = false;

			try
			{
				RunAndThrow();
			}
			catch (Exception e)
			{
				exceptionHandled = true;

				_testOutputHelper.WriteLine($"Exception handled: {e.GetType().Name} - {e.Message}");
			}

			exceptionHandled.Should().BeFalse();
		}

		[Fact]
		public void HandledTaskExceptionTest()
		{
			var exceptionHandled = false;

			try
			{
				RunAndThrow().Wait();
			}
			catch (Exception e)
			{
				exceptionHandled = true;

				_testOutputHelper.WriteLine($"Exception handled: {e.GetType().Name} - {e.Message}");
			}

			exceptionHandled.Should().BeTrue();
		}

		[Fact]
		public void HandledTaskWithContinuationAggregatedExceptionTest()
		{
			var exceptionHandled = false;

			try
			{
				RunTaskWithContinuation().Wait();
			}
			catch (AggregateException e)
			{
				exceptionHandled = true;

				_testOutputHelper.WriteLine($"Exception handled. Exceptions count is {e.InnerExceptions.Count}");
				_testOutputHelper.WriteLine($"Exception {e.InnerExceptions[0].Message}");
			}

			exceptionHandled.Should().BeTrue();
		}

		[Fact]
		public void HandledNestedTasksAggregatedExceptionTest()
		{
			var exceptionHandled = false;

			try
			{
				RunNestedTasks().Wait();
				//RunNestedTasks().GetAwaiter().GetResult();
			}
			catch (AggregateException e)
			{
				exceptionHandled = true;

				_testOutputHelper.WriteLine($"Exception handled. Exceptions count is {e.InnerExceptions.Count}");
				_testOutputHelper.WriteLine($"Exception {e.InnerExceptions[0].Message}");

				//var flatten = e.Flatten();

			}
			catch (Exception e)
			{
				exceptionHandled = true;

				_testOutputHelper.WriteLine($"Exception handled: {e.GetType().Name} - {e.Message}");
			}

			exceptionHandled.Should().BeTrue();
		}

		[Fact]
		public async Task HandledTasksAggregatedExceptionWithAsyncAwaitTest()
		{
			var exceptionHandled = false;

			try
			{
				await RunNestedTasks();
			}
			catch (Exception e)
			{
				exceptionHandled = true;

				_testOutputHelper.WriteLine($"Exception handled: {e.GetType().Name} {e.Message}");
			}

			exceptionHandled.Should().BeTrue();
		}

		private async Task RunAndThrow()
		{
			throw new NotImplementedException();
		}

		private Task RunTaskWithContinuation()
		{
			return Task.Run(() =>
			{
				_testOutputHelper.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				throw new Exception("task1");
			}).ContinueWith(parentTask =>
			{
				_testOutputHelper.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				throw new Exception("task2");
			});
		}

		private Task RunNestedTasks()
		{
			return Task.Factory.StartNew(() =>
			{
				_testOutputHelper.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				Task.Factory.StartNew(() =>
				{
					_testOutputHelper.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
					throw new Exception("nested task");
				}, TaskCreationOptions.AttachedToParent);

				throw new Exception("main task");
			});
		}
	}
}
