using System;
using System.Threading;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex1
{
	public class MultiThreadingWithThreadPoolTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public MultiThreadingWithThreadPoolTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void ThreadPoolExampleTest()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var threadWithState = new ThreadWithState(
				ThreadCallbackHandler,
				_testOutputHelper);

			var result = ThreadPool.QueueUserWorkItem(threadWithState.Execute, new JobData
			{
				Id = Guid.NewGuid()
			});
			
			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");

			result.Should().BeTrue();
		}

		private class JobData
		{
			public Guid Id { get; set; }
		}

		private void ThreadCallbackHandler(Guid? data)
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

			public void Execute(object state)
			{
				var jobData = state as JobData;

				_testOutputHelper.WriteLine($"Process with ID {jobData?.Id} runs on thread {Thread.CurrentThread.ManagedThreadId}");
				_callback?.Invoke(jobData?.Id);
			}
		}
		public delegate void ThreadCallback(Guid? output);
	}
}
