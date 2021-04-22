using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part5
{
	public class ValueTaskTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly HttpClient _httpClient = new HttpClient();
		private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

		public ValueTaskTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task OptimizedAsyncOperationTest()
		{
			var joke1 = await GetJokeWithTask();

			var joke2 = await GetJokeWithValueTask();

			// ValueTask remarks
			// Do not await many times
			var jokeValueTask = GetJokeWithValueTask();
			var jokeA = await jokeValueTask;
			var jokeB = await jokeValueTask;

			// Do not awaits concurrently (and, by definition then, multiple times)
			var jokeValueTask2 = GetJokeWithValueTask();
			var task1 = Task.Run(async () => await jokeValueTask2);
			var task2 =  Task.Run(async () => await jokeValueTask2);
			await Task.WhenAll(task1, task2);

			// Do not block
			var jokeValueTask3 = GetJokeWithValueTask();
			var jokeC = jokeValueTask3.GetAwaiter().GetResult();

			// If above required use following
			var jokeValueTask4 = GetJokeWithValueTask().AsTask();
		}

		private async Task<string> GetJokeWithTask()
		{
			var joke = _cache.Get<string>("cache_joke");

			if (joke is null)
			{
				joke = await _httpClient
					.GetStringAsync("https://api.chucknorris.io/jokes/random");

				_cache.Set("cache_joke", joke, TimeSpan.FromMinutes(1));
			}

			return joke;
		}

		private async ValueTask<string> GetJokeWithValueTask()
		{
			var joke = _cache.Get<string>("cache_joke");

			if (joke is null)
			{
				joke = await _httpClient
					.GetStringAsync("https://api.chucknorris.io/jokes/random");

				_cache.Set("cache_joke", joke, TimeSpan.FromMinutes(1));
			}

			return joke;
		}
	}
}
