using System;
using System.Text.Json;
using System.Threading.Tasks;
using Hangfire.Console;
using Hangfire.Server;
using WebAppDotNet5.Infrastructure.Persistence;
using WebAppDotNet5.Infrastructure.Tracing.Jaeger.Helpers;

namespace WebAppDotNet5.Application.Jobs
{
	public class AnalyzeHazardLevelJob : IJob<WeatherForecastJobData>
	{
		private readonly WeatherForecastDbContext _context;
		private readonly ITracingHelper _tracingHelper;

		public AnalyzeHazardLevelJob(
			WeatherForecastDbContext context,
			ITracingHelper tracingHelper)
		{
			_context = context;
			_tracingHelper = tracingHelper;
		}

		public async Task Execute(WeatherForecastJobData jobData, PerformContext context)
		{
			using var scope = _tracingHelper
				.StartServerSpan(
					jobData.TracingKeys, 
					$"{_tracingHelper.JobOperationActionName}-{nameof(PredictWeatherForecastJob)}");

			try
			{
				_tracingHelper.LogToSpan(nameof(jobData), JsonSerializer.Serialize(jobData));

				context.WriteLine("Analyze hazard level");

				await Task.Delay(1000);
			}
			catch (Exception e)
			{
				_tracingHelper.LogSpanException(e);
				throw;
			}
		}
	}
}