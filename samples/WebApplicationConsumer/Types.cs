using Vogen;

namespace WebApplicationConsumer;

public record WeatherForecast(DateOnly Date, Centigrade TemperatureC, Farenheit TemperatureF, string? Summary, City City)
{
}


[ValueObject<string>]
public partial class City
{
}

[ValueObject]
public partial struct Farenheit
{
}

[ValueObject]
public partial struct Centigrade
{
}

