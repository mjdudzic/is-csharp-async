using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Nito.AsyncEx;
using Tests.Part4.Ex1;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part5
{
	public class GoodPracticesExamplesTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly HttpClient _httpClient = new HttpClient();

		public GoodPracticesExamplesTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task DoNotPretendAsync()
		{
			var result = await DoSomeApiCall();
			
			result.Should().NotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task DoNotAwaitInLoop()
		{
			var queries = new List<string>
			{
				"animal",
				"tree",
				"friends"
			};

			var results = new List<string>();
			foreach (var query in queries)
			{
				var result = await SearchForJoke(query);

				results.Add(result);
			}

			results.Should().NotBeEmpty();
		}

		[Fact]
		public async Task DoNotChangeNonThreadSafeObjectInTask()
		{
			var results = new List<string>();
			var task1 =  SearchForJoke("lion", results);
			var task2 = SearchForJoke("mouse", results);

			await Task.WhenAll(task1, task2);

			results.Should().NotBeEmpty();
		}

		[Fact]
		public async Task DoNotUseAsyncLambdaInForEach()
		{
			var queries = new List<string>
			{
				"animal",
				"tree",
				"friends"
			};

			var results = new List<string>();
			queries.ForEach(async query => results.Add(await SearchForJoke(query)));

			await Task.CompletedTask;

			results.Should().NotBeEmpty();
		}

		[Fact]
		public async Task AwaitInTyrCatchBlock()
		{
			await Task.CompletedTask;
			try
			{
				BadMethod();
			}
			catch (Exception e)
			{
				_testOutputHelper.WriteLine(e.Message);
			}
		}

		[Fact]
		public async Task UseDiscardForFireAndForgetTask()
		{
			await Task.CompletedTask;
			_ = Task.Run(() =>
			{
				Thread.Sleep(1000);
			});
		}

		[Fact]
		public async Task DoNotUseAsyncElidingWithUsingStatement()
		{
			var joke = await SearchForRandomJoke();
			
			joke.Should().NotBeNullOrWhiteSpace();
		}

		private async Task<string> DoSomeApiCall()
		{
			return await Task.Run(() =>
			{
				var webClient = new WebClient();
				return webClient.DownloadString("https://api.chucknorris.io/jokes/random");
			});
		}

		private async Task<string> SearchForJoke(string query)
		{
			return await _httpClient.GetStringAsync($"https://api.chucknorris.io/jokes/search?query={query}");
		}

		private async Task SearchForJoke(string query, List<string> results)
		{
			var result =  await _httpClient.GetStringAsync($"https://api.chucknorris.io/jokes/search?query={query}");

			results.Add(result);
		}

		private Task<string> SearchForRandomJoke()
		{
			using var httpClient = new HttpClient();
			return httpClient.GetStringAsync($"https://api.chucknorris.io/jokes/random");
		}

		private async Task BadMethod()
		{
			await Task.CompletedTask;

			throw new Exception("something bad happened");
		}
	}
}
