using Hangfire;
using Hangfire.Console;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebAppDotNet5.Extensions
{
	internal static class HangfireDependencyInjection
	{
		public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHangfire(config => config
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseConsole()
				.UseSqlServerStorage(configuration["ConnectionStrings:Default"]));

			services.AddHangfireServer();

			return services;
		}

		public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
			=> app.UseHangfireDashboard("/dashboard", new DashboardOptions
			{
				IgnoreAntiforgeryToken = true
			});
	}
}