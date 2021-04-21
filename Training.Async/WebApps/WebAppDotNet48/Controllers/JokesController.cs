using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAppDotNet48.Services;

namespace WebAppDotNet48.Controllers
{
	public class JokesController : ApiController
	{
		private static HttpClient HttpClient = new HttpClient();

		[HttpGet]
		[Route("api/jokes/random")]
		public string Get()
		{
			var service = new JokesService(HttpClient);

			var task = service.GetJokeAsync();

			var joke = task.Result;

			return joke;
		}
	}
}
