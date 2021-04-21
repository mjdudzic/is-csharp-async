using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppDotNet5.Application.Services;
using WebAppDotNet5.Extensions;
using WebAppDotNet5.Infrastructure.Persistence;
using WebAppDotNet5.Infrastructure.Tracing.Jaeger;

namespace WebAppDotNet5
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAppDotNet5", Version = "v1" });
			});

			//services.AddHostedService<IntegrationsHealthCheckHostedService>();

			var connectionString = Configuration["ConnectionStrings:Default"];
			services
				.AddEntityFrameworkSqlServer()
				.AddDbContext<WeatherForecastDbContext>(
				options =>
				{
					options.EnableDetailedErrors();
					options.UseSqlServer(connectionString);
				});

			services.AddHangfire(Configuration);

			services.AddJaeger(Configuration);

			services.AddHttpClient();

			services.AddTransient<IWeatherForecastAnalysisService, WeatherForecastAnalysisService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAppDotNet5 v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseHangfire();

			app.UseJaeger();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
