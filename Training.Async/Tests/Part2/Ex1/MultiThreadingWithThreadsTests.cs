using System;
using System.Threading;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex1
{
	public class MultiThreadingWithThreadsTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public MultiThreadingWithThreadsTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void ThreadExampleTest()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var thread = new Thread(() =>
			{
				//Thread.Sleep(100);
				_testOutputHelper.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} is running");
			});

			thread.Start();

			//thread.Join();

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");

			thread.ThreadState.Should().NotBeEquivalentTo(ThreadState.Aborted);
		}

		[Fact]
		public void ThreadWithParamsAndCallbackExampleTest()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var threadWithState = new ThreadWithState(
				ThreadCallbackHandler,
				_testOutputHelper);

			var thread = new Thread(threadWithState.Execute);

			thread.Start("001");

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");

			thread.ThreadState.Should().NotBeEquivalentTo(ThreadState.Aborted);
		}

		private void ThreadCallbackHandler(string data)
		{
			_testOutputHelper.WriteLine($"Thread callback data is {data}");
		}

		public class ThreadWithState
		{
			private readonly ThreadCallback _callback;
			private readonly ITestOutputHelper _testOutputHelper;

			public ThreadWithState(
				ThreadCallback callbackDelegate,
				ITestOutputHelper testOutputHelper)
			{
				_callback = callbackDelegate;
				_testOutputHelper = testOutputHelper;
			}

			public void Execute(object args)
			{
				var processId = args as string;

				_testOutputHelper.WriteLine($"Process with ID {processId} runs on thread {Thread.CurrentThread.ManagedThreadId}");
				_callback?.Invoke(Guid.NewGuid().ToString("N"));
			}
		}
		public delegate void ThreadCallback(string output);
	}
}
