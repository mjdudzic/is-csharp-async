using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Part5
{
	public class AsyncEnumerableTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly HttpClient _httpClient = new HttpClient();

		public AsyncEnumerableTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task SyncAndAsyncEnumerableTest()
		{
			foreach (var dataItem in await GetDataFromMemory())
			{
				_testOutputHelper.WriteLine($"Request length: {dataItem.Length}");
			}

			foreach (var dataItem in GetDataFromEnumerable())
			{
				_testOutputHelper.WriteLine($"Request length: {dataItem.Length}");
			}

			await foreach (var dataItem in GetDataFromAsyncEnumerable())
			{
				_testOutputHelper.WriteLine($"Request length: {dataItem.Length}");
			}
		}

		public IEnumerable<string> GetDataFromEnumerable()
		{
			var websites = new[] {
				"https://api.chucknorris.io/jokes/search?query=developer",
				"https://api.chucknorris.io/jokes/search?query=programming",
				"https://api.chucknorris.io/jokes/search?query=microsoft"
			};

			foreach (var website in websites)
			{
				var requestTask = _httpClient.GetAsync(website);
				var request = requestTask.GetAwaiter().GetResult(); // Blocking :|
				yield return request.Content.ReadAsStringAsync().Result; // Blocking again :(
			}
		}

		private async Task<IEnumerable<string>> GetDataFromMemory()
		{
			var websites = new[] {
				"https://api.chucknorris.io/jokes/search?query=developer",
				"https://api.chucknorris.io/jokes/search?query=programming",
				"https://api.chucknorris.io/jokes/search?query=microsoft"
			};

			var list = new List<string>();

			foreach (var website in websites)
			{
				var resp = await _httpClient.GetAsync(website);
				list.Add(await resp.Content.ReadAsStringAsync());
			}

			return list;
		}

		private async IAsyncEnumerable<string> GetDataFromAsyncEnumerable()
		{
			var websites = new[] {
				"https://api.chucknorris.io/jokes/search?query=developer",
				"https://api.chucknorris.io/jokes/search?query=programming",
				"https://api.chucknorris.io/jokes/search?query=microsoft"
			};

			foreach (var website in websites)
			{
				var req = await _httpClient.GetAsync(website);
				yield return await req.Content.ReadAsStringAsync();
			}
		}

		private async Task WriteToFile(string filePath)
		{
			await using var fs = new FileStream(filePath, FileMode.Create);
			await using var sw = new StreamWriter(fs);
			
			await sw.WriteAsync("Welcome in async world");
		}
	}
}
