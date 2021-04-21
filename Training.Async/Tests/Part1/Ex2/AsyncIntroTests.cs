using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part1.Ex2
{
	public class AsyncIntroTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public AsyncIntroTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task AsyncTest()
		{
			_testOutputHelper.WriteLine($"Starting program - Thread {Thread.CurrentThread.ManagedThreadId}");

			var dataService = new DataService(
				Path.Combine(Directory.GetCurrentDirectory(), "TestData"),
				_testOutputHelper);

			var dataFromFile1Task = dataService.GetData("test1.txt");
			var dataFromFile2Task = dataService.GetData("test2.txt");

			var dataFromFile2 = await dataFromFile2Task;
			var dataFromFile1 = await dataFromFile1Task;
			
			_testOutputHelper.WriteLine($"File 1 contains text with length: {dataFromFile1.Length}");
			_testOutputHelper.WriteLine($"File 2 contains text with length: {dataFromFile2.Length}");

			_testOutputHelper.WriteLine($"Closing program - Thread {Thread.CurrentThread.ManagedThreadId}");

			dataFromFile1Task.IsCompleted.Should().BeTrue();
		}

	}
}
