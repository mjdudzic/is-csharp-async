using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppDotNet5.Application.Services;
using WebAppDotNet5.Domain;
using WebAppDotNet5.Infrastructure.Persistence;

namespace WebAppDotNet5.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly WeatherForecastDbContext _context;
		private readonly IWeatherForecastAnalysisService _weatherForecastAnalysisService;
		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(
			WeatherForecastDbContext context,
			IWeatherForecastAnalysisService weatherForecastAnalysisService,
			ILogger<WeatherForecastController> logger)
		{
			_context = context;
			_weatherForecastAnalysisService = weatherForecastAnalysisService;
			_logger = logger;
		}

		[HttpGet]
		public IEnumerable<WeatherForecast> Get()
		{
			return _context
				.WeatherForecasts
				.OrderBy(i => i.Date)
				.ToList();
		}

		[HttpGet("{id}")]
		public async Task<WeatherForecast> GetById(int id)
		{
			return await _context
				.WeatherForecasts
				.FindAsync(id);
		}

		[HttpPost]
		public async Task<IActionResult> Post(WeatherForecast weatherForecast)
		{
			await _weatherForecastAnalysisService.ProcessData(weatherForecast);

			return CreatedAtAction(nameof(GetById), new { id = weatherForecast.Id }, weatherForecast);
		}
	}
}
