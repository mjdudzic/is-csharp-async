using System.Net.Http;
using System.Threading.Tasks;

namespace Tests.Part4.Ex1
{
	public class DataService
	{
		public async Task<string> GetJokeAsync()
		{
			using var client = new HttpClient();

			return await client
				.GetStringAsync("https://api.chucknorris.io/jokes/random");
		}
	}

	public class DataService2
	{
		public async Task<string> GetJokeAsync()
		{
			return await new DataService().GetJokeAsync();
		}
	}
}