using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Library;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part1.Ex3
{
	public class AsyncComplexStuffsTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public AsyncComplexStuffsTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public void EnumerableStateMachineExampleTest()
		{
			var resourceCollection = new AppResourceCollection();

			foreach (var resource in resourceCollection)
			{
				_testOutputHelper.WriteLine($"Resource is {resource}");
			}

			resourceCollection.Any().Should().BeTrue();
		}

		[Fact]
		public async Task AsyncStateMachineExampleTest()
		{
			try
			{
				var resourceHelper = new ResourceHelper(new HttpClient());

				var uri = new Uri("https://api.chucknorris.io/jokes/random");
				var filePath = Path.Combine(
					Directory.GetCurrentDirectory(),
					"TestData",
					"Temp",
					$"resource-{DateTime.Now.ToLongDateString()}.txt");

				await resourceHelper.SaveResource(uri, filePath);

				File.Exists(filePath).Should().BeTrue();
			}
			catch (Exception e)
			{
				_testOutputHelper.WriteLine(e.Message);
				throw;
			}
		}

		[Fact]
		public async Task TestAwaiterExampleTest()
		{
			try
			{
				var testAwaitable = new TestAwaitable(_testOutputHelper);

				await testAwaitable;

				testAwaitable.GetAwaiter().IsCompleted.Should().BeTrue();
			}
			catch (Exception e)
			{
				_testOutputHelper.WriteLine(e.Message);
				throw;
			}
		}

		[Fact]
		public async Task JokeAwaiterExampleTest()
		{
			try
			{
				var jokeAwaitable = new JokeAwaitable(new HttpClient(), _testOutputHelper);
				
				var joke = await jokeAwaitable;

				joke.Should().NotBeNullOrWhiteSpace();
			}
			catch (Exception e)
			{
				_testOutputHelper.WriteLine(e.Message);
				throw;
			}
		}

		[Fact]
		public async Task TaskCultureAwaiterExampleTest()
		{
			try
			{
				_testOutputHelper.WriteLine(
					$"Current thread ID {Thread.CurrentThread.ManagedThreadId} with culture {Thread.CurrentThread.CurrentCulture.Name}");

				Thread.CurrentThread.CurrentCulture = new CultureInfo("pl-PL");

				_testOutputHelper.WriteLine(
					$"After change current thread ID {Thread.CurrentThread.ManagedThreadId} with culture {Thread.CurrentThread.CurrentCulture.Name}");

				var currentCultureName = Thread.CurrentThread.CurrentCulture.Name;

				await Task.Run(() =>
				{
					_testOutputHelper.WriteLine(
						$"Async thread ID {Thread.CurrentThread.ManagedThreadId} with culture {Thread.CurrentThread.CurrentCulture.Name}");
				}).WithCurrentCulture();

				_testOutputHelper.WriteLine(
					$"Current thread ID {Thread.CurrentThread.ManagedThreadId} with culture {Thread.CurrentThread.CurrentCulture.Name}");

				Thread.CurrentThread.CurrentCulture.Name.Should().BeEquivalentTo(currentCultureName);
			}
			catch (Exception e)
			{
				_testOutputHelper.WriteLine(e.Message);
				throw;
			}
		}

		[Fact]
		public async Task TimeSpanAwaiterExampleTest()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			//await Task.Delay(1000);
			await TimeSpan.FromSeconds(1);
			await 1000;
			await DateTimeOffset.UtcNow.AddSeconds(1);

			stopwatch.Stop();
			_testOutputHelper.WriteLine($"Test lasted {stopwatch.ElapsedMilliseconds} ms");

			stopwatch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(3000);
		}

		[Fact]
		public async Task ProcessAwaiterExampleTest()
		{
			var result = await Process.Start("cmd.exe", "/c ping infoshareacademy.com");
			_testOutputHelper.WriteLine($"Command exited with code {result}");

			result.Should().Be(0);
		}
	}
}
