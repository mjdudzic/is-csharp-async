using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Timer = System.Threading.Timer;

namespace WebAppDotNet5.Application.Services
{
	public class IntegrationsHealthCheckHostedService : IHostedService, IDisposable
	{
		private int _executionCount = 0;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<IntegrationsHealthCheckHostedService> _logger;
		private Timer _timer;
		private AsyncRetryPolicy _retryPolicy;

		public IntegrationsHealthCheckHostedService(
			IHttpClientFactory httpClientFactory,
			ILogger<IntegrationsHealthCheckHostedService> logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;

			_retryPolicy = Policy
				.Handle<Exception>()
				.WaitAndRetryAsync(2, retryAttempt => {
						var timeToWait = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
						
						_logger.LogInformation($"Retry after {timeToWait.TotalSeconds} seconds");
						
						return timeToWait;
					}
				);
		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Timed Hosted Service running.");

			_timer = new Timer(DoHealthChecks, null, TimeSpan.Zero,
				TimeSpan.FromSeconds(100));

			return Task.CompletedTask;
		}

		private void DoHealthChecks(object state)
		{
			var count = Interlocked.Increment(ref _executionCount);

			try
			{
				_retryPolicy.ExecuteAsync(async () => await TestHealthChecks()).Wait();
			}
			catch (Exception e)
			{
				_logger.LogWarning("Integration fails -> sending notifications");
			}

			_logger.LogInformation(
				"Timed Hosted Service is working. Count: {Count}", count);
		}

		public Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Timed Hosted Service is stopping.");

			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}

		private async Task TestHealthChecks()
		{
			var httpClient = _httpClientFactory.CreateClient();

			var result = await httpClient.GetAsync(new Uri("https://postman-echo.com/delay/1"));

			throw new Exception("test");

			result.EnsureSuccessStatusCode();
		}
	}
}