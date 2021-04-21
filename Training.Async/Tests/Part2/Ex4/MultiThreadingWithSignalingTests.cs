using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part2.Ex4
{
	public class MultiThreadingWithSignalingTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		private readonly EventWaitHandle _autoResetEvent =
			new EventWaitHandle(true, EventResetMode.AutoReset);

		private readonly Fixture _fixture;

		private readonly List<string> _data;

		public MultiThreadingWithSignalingTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
			_fixture = new Fixture();
			_data = _fixture.CreateMany<string>(10).ToList();
		}

		[Fact]
		public void MultiThreadingWithoutLockExampleTest()
		{
			Parallel.For(0, 10, (index, state) =>
			{
				_autoResetEvent.WaitOne();

				Thread.Sleep(1000); //do some work

				var item = _data[new Random().Next(0, 9)];
				
				_testOutputHelper.WriteLine(
					$"Thread {Thread.CurrentThread.ManagedThreadId} has read {item}");

				_autoResetEvent.Set();
			});

			_autoResetEvent.Set();
		}
	}
}
