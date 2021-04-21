using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebAppDotNet5.Domain;

namespace WebAppDotNet5.Infrastructure.Persistence
{
	public class WeatherForecastDbContext : DbContext
	{
		public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options)
			: base(options)
		{
		}

		public virtual DbSet<WeatherForecast> WeatherForecasts { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
			=> modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}