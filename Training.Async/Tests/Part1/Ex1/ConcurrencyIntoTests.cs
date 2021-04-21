using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part1.Ex1
{
	public class ConcurrencyIntoTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public ConcurrencyIntoTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task ConcurrencyTest()
		{
			_testOutputHelper.WriteLine("*** Concurrency examples ***");
			_testOutputHelper.WriteLine($"Program start - Thread {Thread.CurrentThread.ManagedThreadId}");
			_testOutputHelper.WriteLine("");

			await MultiThreading();

			_testOutputHelper.WriteLine("");

			await Parallelism();

			_testOutputHelper.WriteLine("");

			_testOutputHelper.WriteLine("--- Async example ---");
			var async1 = Async(1);
			var async2 = Async(2);

			await Task.WhenAll(async1, async2);

			async1.IsCompleted.Should().BeTrue();
			async2.IsCompleted.Should().BeTrue();

			_testOutputHelper.WriteLine("");
			_testOutputHelper.WriteLine($"Program end - Thread {Thread.CurrentThread.ManagedThreadId}");
		}

		public Task MultiThreading()
		{
			_testOutputHelper.WriteLine("--- MultiThreading example ---");

			// Threads are doing different job at the same time
			var readQueueTask = Task.Run(() =>
			{
				_testOutputHelper.WriteLine($"Reading queue - Thread {Thread.CurrentThread.ManagedThreadId}");
			});

			var sendNotificationsTask = Task.Run(() =>
			{
				_testOutputHelper.WriteLine($"Sending notifications - Thread {Thread.CurrentThread.ManagedThreadId}");
			});

			var integrationHealthCheckingTask = Task.Run(() =>
			{
				_testOutputHelper.WriteLine($"Health checking - Thread {Thread.CurrentThread.ManagedThreadId}");
			});

			return Task.WhenAll(readQueueTask, sendNotificationsTask, integrationHealthCheckingTask);
		}

		public Task Parallelism()
		{
			// Threads are used to complete one job faster (e.g. many sub-calculations)
			// E.g. we have two waitress taking orders

			_testOutputHelper.WriteLine("--- Parallelism example ---");

			var files = new[] { "file1", "file2", "file3" };

			return Task.Run(() =>
			{
				Parallel.ForEach(files, file =>
				{
					_testOutputHelper.WriteLine($"Processing file {file} - Thread {Thread.CurrentThread.ManagedThreadId}");
				});
			});
		}

		public async Task Async(int id)
		{
			// Thread is not blocked if in idle state

			_testOutputHelper.WriteLine($"--- Async job {id} ---");
			_testOutputHelper.WriteLine($"Async {id} - Reading file - Thread {Thread.CurrentThread.ManagedThreadId}");

			await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "TestData", "test1.txt"));
		}
	}
}
