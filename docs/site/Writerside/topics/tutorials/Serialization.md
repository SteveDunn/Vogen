# Working with ASP.NET Core

In this tutorial, we'll create a type and see how to serialize and deserialize it.

First, create a value object:

```C#
[ValueObject<int>]
public partial struct CustomerId { }
```

In the `ValueObject`, we don't specify any additional configuration other than the underlying primitive that is
being wrapped.
We'll see shortly how to add this configuration, but for now, we'll look at the default configuration
and what it allows us to do.

By default, each value object generates a `TypeConverter` and `System.Text.Json` (STJ) serializer.
We can see this from the following snippet of generated code:

```C#
[global::System.Text.Json.Serialization.JsonConverter(typeof(CustomerIdSystemTextJsonConverter))]
[global::System.ComponentModel.TypeConverter(typeof(CustomerIdTypeConverter))]
public partial struct CustomerId
{
} 
```

'Type Converters' are used by the .NET runtime in things like ASP.NET Core Web Applications, grids, and WPF bindings. 
Let's see how that works in a web API project.

Create a new ASP.NET Core Web API targeting .NET 7.0. In the project, look at the endpoint named `GetWeatherForecast` in
the `WeatherForecastController` type. It looks like this:

```C#
[HttpGet(Name = "GetWeatherForecast")]
public IEnumerable<WeatherForecast> Get()
{
    ...
}
```

To demonstrate how we can use Value Objects, we'll add a `CityName` value object and use it as part of the request.

Create a `CityName` type:

```c#
[ValueObject<string>]
public partial struct CityName { }
```

... now, add this as a parameter to the endpoint:
```C#
    [HttpGet("/WeatherForecast/{cityName}")]
    public IEnumerable<WeatherForecast> Get(CityName cityName)
    {
    }
```

Add a `CityName` property to the `WeatherForecast` type:

```C#
public class WeatherForecast
{
    public CityName CityName { get; set; }
    
    public DateOnly Date { get; set; }

    ...
```

Now, change the code in the endpoint to populate the new `CityName` property.
We're keeping things simple and just returning what we were provided:

```c#
return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        + CityName = cityName,
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    })
    .ToArray();
```

You can now access that via the URL and treat the value object as a string,
e.g. `http://localhost:5053/WeatherForecast/London`

You should see the City returned, something like this:

<img border-effect="rounded" alt="return-from-web-api.png" src="return-from-web-api.png"/>


<note>
You'll need to replace for 5053 to whatever is specified in the project just created, which can be 
found in launchSettings.json.

Also, you'll see that Swagger treats cityName as a JSON field. This can be changed as detailed in [this How-to article](Use-in-Swagger.md).

</note>

