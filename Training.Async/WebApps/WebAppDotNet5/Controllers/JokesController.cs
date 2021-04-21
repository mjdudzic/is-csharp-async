using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppDotNet5.Application.Services;
using WebAppDotNet5.Infrastructure.Persistence;

namespace WebAppDotNet5.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class JokesController : ControllerBase
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<JokesController> _logger;
		private readonly WeatherForecastDbContext _dbContext;

		public JokesController(
			IHttpClientFactory httpClientFactory,
			ILogger<JokesController> logger,
			WeatherForecastDbContext dbContext)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
			_dbContext = dbContext;
		}

		[HttpGet("random")]
		public string Get()
		{
			var service = new JokesService(_httpClientFactory.CreateClient());

			var task = service.GetJokeAsync();

			var joke = task.Result;

			return joke;
		}

		[HttpPost]
		public async Task<IActionResult> Post(JokeData data)
		{
			var service = new AppService(_httpClientFactory.CreateClient(), _dbContext);

			//var jokeApiCallTask = service.GetJokeAsync();

			//var externalApiCallTask = service.CallExternalApi();

			//var dbCallTask = service.GetDbData();

			//await Task.WhenAll(
			//	jokeApiCallTask,
			//	externalApiCallTask,
			//	dbCallTask);

			var jokeApiCallTask = await service.GetJokeAsync();

			var externalApiCallTask = await service.CallExternalApi();

			var dbCallTask = await service.GetDbData();

			return Ok();
		}
	}

	public class JokeData
	{
		public string Text { get; set; }
	}
}
