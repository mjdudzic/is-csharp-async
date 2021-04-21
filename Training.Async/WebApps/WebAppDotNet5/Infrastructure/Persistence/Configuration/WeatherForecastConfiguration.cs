using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAppDotNet5.Domain;

namespace WebAppDotNet5.Infrastructure.Persistence.Configuration
{
	public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecast>
	{
		public void Configure(EntityTypeBuilder<WeatherForecast> builder)
		{
			builder.ToTable("WeatherForecast");
		}
	}
}