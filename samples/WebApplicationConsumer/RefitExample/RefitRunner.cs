using Refit;

namespace RefitExample;

public static class RefitRunner
{
    public static async Task Run()
    {
        Console.WriteLine("Refit example");
        Console.WriteLine("=============");
        var api = RestService.For<IJsonPlaceholderApi>("https://localhost:7033");

        await GetWeatherForecast(City.From("London"));
        Console.WriteLine("=============");

        await GetWeatherForecast(City.From("Paris"));
        Console.WriteLine("=============");

        await GetWeatherForecast(City.From("Peckham"));
        Console.WriteLine("=============");
        
        return;

        async Task GetWeatherForecast(City city)
        {
            try
            {

                var forecasts = await api.GetWeatherForecastByCity(city);
                foreach (var f in forecasts)
                {
                    Console.WriteLine($"City: {f.City}, TempC: {f.TemperatureC} ({f.TemperatureF.Value}F) - {f.Summary}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

public interface IJsonPlaceholderApi
{
    [Get("/weatherforecast")]
    Task<List<WeatherForecast>> GetWeatherForecast();

    [Get("/weatherforecast/{city}")]
    Task<List<WeatherForecast>> GetWeatherForecastByCity(City city);
}
