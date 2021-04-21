using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppDotNet5.Domain;
using WebAppDotNet5.Infrastructure.Persistence;

namespace WebAppDotNet5.Application.Services
{
	public class AppService
	{
		private readonly HttpClient _httpClient;
		private readonly WeatherForecastDbContext _context;

		public AppService(
			HttpClient httpClient,
			WeatherForecastDbContext context)
		{
			_httpClient = httpClient;
			_context = context;
		}

		public async Task<string> GetJokeAsync()
		{
			return await _httpClient
					.GetStringAsync(new Uri("https://api.chucknorris.io/jokes/random"));
		}

		public async Task<HttpResponseMessage> CallExternalApi()
		{
			return await _httpClient.GetAsync(new Uri("https://postman-echo.com/delay/1"));
		}

		public async Task<List<WeatherForecast>> GetDbData()
		{
			return await _context
				.WeatherForecasts
				.OrderByDescending(i => i.Date)
				.Take(3)
				.ToListAsync();
		}
	}
}