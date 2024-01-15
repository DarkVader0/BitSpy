namespace BitSpy.Api.Generator.Services;

public interface IWeatherForecastService
{
    Task<List<WeatherForecast>> GetWeatherForecastAsync(CancellationToken cancellationToken = default);
}