using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace ValueTaskWithBenchmarkDemoApp
{
	public class DataService
	{
		private static readonly HttpClient HttpClient = new HttpClient();
		private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

		public async Task<string> GetJokeWithTask()
		{
			var joke = _cache.Get<string>("cache_joke");

			if (joke is null)
			{
				joke = await HttpClient
					.GetStringAsync("https://api.chucknorris.io/jokes/random");

				_cache.Set("cache_joke", joke, TimeSpan.FromMinutes(1));
			}

			return joke;
		}

		public async ValueTask<string> GetJokeWithValueTask()
		{
			var joke = _cache.Get<string>("cache_joke");

			if (joke is null)
			{
				joke = await HttpClient
					.GetStringAsync("https://api.chucknorris.io/jokes/random");

				_cache.Set("cache_joke", joke, TimeSpan.FromMinutes(1));
			}

			return joke;
		}
	}
}