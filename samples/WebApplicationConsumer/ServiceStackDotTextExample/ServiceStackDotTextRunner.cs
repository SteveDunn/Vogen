using ServiceStack;
using WebApplicationConsumer;

namespace ServiceStackDotTextExample;

public static class ServiceStackTextRunner
{
    public static async Task Run()
    {
        Console.WriteLine("ServiceStack.Text example");
        Console.WriteLine("=============");

        string json = await "https://localhost:7033/weatherforecast/London"
            .GetJsonFromUrlAsync(req => req.With(x => x.UserAgent = "sample.app"));
        
        WeatherForecast[]? forecasts = json.FromJson<WeatherForecast[]>();
        
        foreach (var f in forecasts)
        {
            Console.WriteLine($"City: {f.City}, TempC: {f.TemperatureC} ({f.TemperatureF.Value}F) - {f.Summary}");
        }
    }
}

