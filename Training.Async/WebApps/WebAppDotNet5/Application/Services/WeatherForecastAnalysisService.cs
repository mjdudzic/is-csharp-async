using System.Threading.Tasks;
using Hangfire;
using WebAppDotNet5.Application.Jobs;
using WebAppDotNet5.Domain;
using WebAppDotNet5.Infrastructure.Persistence;
using WebAppDotNet5.Infrastructure.Tracing.Jaeger.Helpers;

namespace WebAppDotNet5.Application.Services
{
	public class WeatherForecastAnalysisService : IWeatherForecastAnalysisService
	{
		private readonly WeatherForecastDbContext _context;
		private readonly IBackgroundJobClient _backgroundJobClient;
		private readonly ITracingHelper _tracingHelper;

		public WeatherForecastAnalysisService(
			WeatherForecastDbContext context,
			IBackgroundJobClient backgroundJobClient,
			ITracingHelper tracingHelper)
		{
			_context = context;
			_backgroundJobClient = backgroundJobClient;
			_tracingHelper = tracingHelper;
		}

		public async Task ProcessData(WeatherForecast weatherForecast)
		{
			_context.WeatherForecasts.Add(weatherForecast);

			await _context.SaveChangesAsync();

			var jobData = new WeatherForecastJobData
			{
				WeatherForecastId = weatherForecast.Id,
				TracingKeys = _tracingHelper.GetActiveSpanTracingKeys()
			};

			var predictionJobId = _backgroundJobClient
				.Enqueue<PredictWeatherForecastJob>(
					job => job.Execute(jobData, null));

			var riskAnalysisJob = _backgroundJobClient
				.ContinueJobWith<AnalyzeHazardLevelJob>(
					predictionJobId,
					job => job.Execute(jobData, null));
		}
	}
}