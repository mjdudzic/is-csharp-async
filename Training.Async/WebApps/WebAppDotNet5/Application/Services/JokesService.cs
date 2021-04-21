using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebAppDotNet5.Application.Services
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
					.GetStringAsync(new Uri("https://api.chucknorris.io/jokes/random"));
		}
	}

	public interface IJokesService
	{
		Task<string> GetJokeAsync();
	}
}