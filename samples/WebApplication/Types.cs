using Vogen;

record WeatherForecast(ForecastDate Date, Centigrade TemperatureC, Farenheit TemperatureF, string? Summary, City City);

[ValueObject<DateOnly>]
public partial struct ForecastDate;

[ValueObject<string>]
public partial struct City;

[ValueObject<float>]
public partial struct Farenheit
{
    public static Farenheit FromCentigrade(Centigrade c) => From(32 + (int)(c.Value / 0.5556));
}

[ValueObject]
public partial struct Centigrade;

[ValueObject<string>]
public partial class CustomerName
{
}

[ValueObject<int>]
public partial struct OrderId
{
}

public class Order
{
    public OrderId OrderId { get; init; } 

    public CustomerName CustomerName { get; init; } = CustomerName.From("");
}

