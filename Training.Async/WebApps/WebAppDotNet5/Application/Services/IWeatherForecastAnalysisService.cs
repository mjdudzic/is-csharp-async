using System.Threading.Tasks;
using WebAppDotNet5.Domain;

namespace WebAppDotNet5.Application.Services
{
	public interface IWeatherForecastAnalysisService
	{
		Task ProcessData(WeatherForecast weatherForecast);
	}
}