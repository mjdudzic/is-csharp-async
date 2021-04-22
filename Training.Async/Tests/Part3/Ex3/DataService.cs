using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests.Part3.Ex3
{
	public class DataService : IDataService
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public DataService(
			ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		public string GetJoke()
		{
			using var client = new WebClient();

			return client.DownloadString("https://api.chucknorris.io/jokes/random");
		}

		public async Task<string> GetJokeAsync()
		{
			using var client = new WebClient();
			
			//.ConfigureAwait does not work with WebClient (due to EAP)

			return await client
				.DownloadStringTaskAsync("https://api.chucknorris.io/jokes/random");
		}

		public async Task<string> GetJokeAsync2()
		{
			using var client = new HttpClient();

			return await client
				.GetStringAsync("https://api.chucknorris.io/jokes/random");
		}

		public string GetJoke2()
		{
			return GetJokeAsync2().GetAwaiter().GetResult();
		}
	}

	public interface IDataService
	{
		string GetJoke();

		Task<string> GetJokeAsync();
	}
}