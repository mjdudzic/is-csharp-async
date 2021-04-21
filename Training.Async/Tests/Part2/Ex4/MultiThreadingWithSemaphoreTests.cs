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
	public class MultiThreadingWithSemaphoreTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3, 3);

		private readonly Fixture _fixture;

		private readonly List<string> _data;

		public MultiThreadingWithSemaphoreTests(ITestOutputHelper testOutputHelper)
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
				try
				{
					_semaphore.Wait();

					Thread.Sleep(1000);

					var item = _data[new Random().Next(0, 9)];

					_testOutputHelper.WriteLine(
						$"Thread {Thread.CurrentThread.ManagedThreadId} has read {item}");
				}
				finally
				{
					_semaphore.Release();
				}
			});
		}
	}
}
