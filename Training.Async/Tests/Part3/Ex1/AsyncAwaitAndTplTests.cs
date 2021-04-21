using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part3.Ex1
{
	public class AsyncAwaitAndTplTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly HttpClient _httpClient = new HttpClient();

		public AsyncAwaitAndTplTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task ProcessWithAsyncAwait()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var uri = new Uri("https://api.chucknorris.io/jokes/random");
			var filePath = Path.Combine(
				Directory.GetCurrentDirectory(),
				"TestData",
				"Temp",
				$"resource-{DateTime.Now.ToLongDateString()}.txt");

			_testOutputHelper.WriteLine($"Reading API - Thread {Thread.CurrentThread.ManagedThreadId}");

			var data = await _httpClient.GetStringAsync(uri);

			_testOutputHelper.WriteLine($"Saving data in file - Thread {Thread.CurrentThread.ManagedThreadId}");

			await File.WriteAllTextAsync(filePath, data);

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");
		}

		[Fact]
		public void ProcessWithTplOnly()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var uri = new Uri("https://api.chucknorris.io/jokes/random");
			var filePath = Path.Combine(
				Directory.GetCurrentDirectory(),
				"TestData",
				"Temp",
				$"resource-{DateTime.Now.ToLongDateString()}.txt");

			var task = Task.Run(() =>
				{
					_testOutputHelper.WriteLine($"Reading API - Thread {Thread.CurrentThread.ManagedThreadId}");

					return _httpClient.GetStringAsync(uri).Result;
				})
				.ContinueWith(parentTask =>
				{
					_testOutputHelper.WriteLine($"Saving data in file - Thread {Thread.CurrentThread.ManagedThreadId}");

					if (parentTask.Exception != null)
					{
						throw parentTask.Exception;
					}

					File.WriteAllTextAsync(filePath, parentTask.Result).Wait();
				});

			task.Wait();

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");
		}

		[Fact]
		public async Task ProcessWithAsyncAwaitAndTaskRun()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var uri = new Uri("https://api.chucknorris.io/jokes/random");
			var filePath = Path.Combine(
				Directory.GetCurrentDirectory(),
				"TestData",
				"Temp",
				$"resource-{DateTime.Now.ToLongDateString()}.txt");

			var result = await Task.Run(async () =>
			{
				_testOutputHelper.WriteLine($"Reading API - Thread {Thread.CurrentThread.ManagedThreadId}");

				var data = await _httpClient.GetStringAsync(uri);

				_testOutputHelper.WriteLine($"Saving data in file - Thread {Thread.CurrentThread.ManagedThreadId}");

				await File.WriteAllTextAsync(filePath, data);

				return true;
			});

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");

			result.Should().BeTrue();
		}

		[Fact]
		public async Task ProcessWithAsyncAwaitAndTaskFactory()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var uri = new Uri("https://api.chucknorris.io/jokes/random");
			var filePath = Path.Combine(
				Directory.GetCurrentDirectory(),
				"TestData",
				"Temp",
				$"resource-{DateTime.Now.ToLongDateString()}.txt");

			var result = await await Task.Factory.StartNew(async () =>
			{
				_testOutputHelper.WriteLine($"Reading API - Thread {Thread.CurrentThread.ManagedThreadId}");

				var data = await _httpClient.GetStringAsync(uri);

				_testOutputHelper.WriteLine($"Saving data in file - Thread {Thread.CurrentThread.ManagedThreadId}");

				await File.WriteAllTextAsync(filePath, data);

				return true;
			});

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");

			result.Should().BeTrue();
		}

		[Fact]
		public async Task LongRunningTaskWithAsyncAsBadExample()
		{
			_testOutputHelper.WriteLine($"Starting test - Thread {Thread.CurrentThread.ManagedThreadId}");

			var uri = new Uri("https://api.chucknorris.io/jokes/random");
			var filePath = Path.Combine(
				Directory.GetCurrentDirectory(),
				"TestData",
				"Temp",
				$"resource-{DateTime.Now.ToLongDateString()}.txt");

			var result = await Task.Factory.StartNew(async () =>
			{
				_testOutputHelper.WriteLine($"Reading API - Thread {Thread.CurrentThread.ManagedThreadId}");

				var data = await _httpClient.GetStringAsync(uri);

				_testOutputHelper.WriteLine($"Saving data in file - Thread {Thread.CurrentThread.ManagedThreadId}");

				await File.WriteAllTextAsync(filePath, data);

				return true;
			}, TaskCreationOptions.LongRunning).Unwrap();

			_testOutputHelper.WriteLine($"Ending test - Thread {Thread.CurrentThread.ManagedThreadId}");

			result.Should().BeTrue();
		}

		[Fact]
		public async Task ProcessTasksAsTheyCompleted()
		{
			var tasks = new List<Task<string>>();
			var data = new List<string>();

			Enumerable.Range(1, 3)
				.ToList()
				.ForEach(i => tasks.Add(_httpClient.GetStringAsync("https://postman-echo.com/delay/1")));

			while (tasks.Any())
			{
				var completedTask = await Task.WhenAny(tasks);
				tasks.Remove(completedTask);

				data.Add(await completedTask);
			}

			data.Count.Should().BeGreaterThan(0);
		}
	}
}
