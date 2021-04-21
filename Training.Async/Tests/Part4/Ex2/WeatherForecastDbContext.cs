using Microsoft.EntityFrameworkCore;

namespace Tests.Part4.Ex2
{
	public class WeatherForecastDbContext : DbContext
	{
		public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options)
			: base(options)
		{
		}

		public virtual DbSet<WeatherForecast> WeatherForecasts { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<WeatherForecast>()
				.ToTable("WeatherForecast");
		}
	}
}