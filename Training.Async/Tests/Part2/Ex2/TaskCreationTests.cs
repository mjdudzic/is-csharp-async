using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex2
{
	public class TaskCreationTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public TaskCreationTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task CreatingTaskByConstructor()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var taskVoid = new Task(() =>
			{
				_testOutputHelper.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
			});

			var taskWithValue = new Task<int?>(() =>
			{
				_testOutputHelper.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				return Task.CurrentId;
			});

			taskVoid.Start();
			taskWithValue.Start();

			//taskVoid.Wait();
			//Task.WaitAll(taskVoid, taskWithValue);
			await Task.WhenAll(taskVoid, taskWithValue);

			var result = taskWithValue.Result;

			taskVoid.IsCompleted.Should().BeTrue();
			result.Should().NotBeNull();
		}

		[Fact]
		public async Task CreatingTaskByFactory()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var taskVoid = Task.Factory.StartNew(() =>
			{
				_testOutputHelper.WriteLine($"Starting task1 {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
			});

			var taskWithValue = Task<int?>.Factory.StartNew(() =>
			{
				_testOutputHelper.WriteLine($"Starting task2 {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				return Task.CurrentId;
			});

			var taskComplex = Task.Factory.StartNew(
				() =>
				{
					_testOutputHelper.WriteLine(
						$"Starting task3 {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
				},
				CancellationToken.None,
				TaskCreationOptions.DenyChildAttach,
				TaskScheduler.Default);

			await Task.WhenAll(taskVoid, taskWithValue, taskComplex);

			var result = taskWithValue.Result;

			taskVoid.IsCompleted.Should().BeTrue();
			result.Should().NotBeNull();
		}

		[Fact]
		public async Task CreatingTaskWithRunMethod()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var taskVoid = Task.Run(() =>
			{
				_testOutputHelper.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");
			});

			var taskWithValue = Task.Run(() =>
			{
				_testOutputHelper.WriteLine($"Starting task {Task.CurrentId} in thread {Thread.CurrentThread.ManagedThreadId}");

				return Task.CurrentId;
			});

			await Task.WhenAll(taskVoid, taskWithValue);

			var result = taskWithValue.Result;

			taskVoid.IsCompleted.Should().BeTrue();
			result.Should().NotBeNull();
		}
	}
}
