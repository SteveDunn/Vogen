# Working with ASP.NET Core

In this tutorial, we'll create a value object and use it an ASP.NET Core Web API project.

We'll see how the .NET Runtime uses 'Type Converters' to convert from primitives that are provided
by the routing framework into Value Objects that you can then use in your domain code

Type Converters are used by the .NET runtime in things like UI grids, WPF bindings, and ASP.NET Core application. 

We'll focus on the latter in this tutorial by creating a new ASP.NET Core Web API and extend the 'weather forecast'
endpoint to take a Value Object representing a city name.

Create a new ASP.NET Core Web API targeting .NET 7.0. 

Next, in the new project, look at the endpoint named `GetWeatherForecast` in the `WeatherForecastController` type.

It looks like this:

```C#
[HttpGet(Name = "GetWeatherForecast")]
public IEnumerable<WeatherForecast> Get()
{
    ...
}
```

Now, add a `CityName` value object and use it as part of the request.

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
        CityName = cityName,
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

## How it works
Vogen, by default, generates a `TypeConverter` for every value object.

We can see this from the following snippet of generated code for the `CityName` value object that we created:

```C#
[TypeConverter(typeof(CityNameTypeConverter))]
public partial struct CityName
{
} 
```

When a request is received, the ASP.NET Core routing framework looks at the `cityName`
parameter to see if it has any type converters that can convert from the primitive
(in this case, a `string`) into a `CityName` type.

It then calls the generated type converter's `ConvertTo` method with a `string`.
The type converter then just calls the `From` method
with that string and returns a `CityName` instance
(after the usual [normalization](NormalizationTutorial.md) and [validation](ValidationTutorial.md) steps).