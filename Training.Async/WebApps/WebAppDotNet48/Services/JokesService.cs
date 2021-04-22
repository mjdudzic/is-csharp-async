using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebAppDotNet48.Services
{
	public class JokesService : IJokesService
	{
		private readonly HttpClient _httpClient;

		public JokesService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<string> GetJokeAsync()
		{
			return await _httpClient
					.GetStringAsync("https://api.chucknorris.io/jokes/random");
		}
	}

	public interface IJokesService
	{
		Task<string> GetJokeAsync();
	}
}