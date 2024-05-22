using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Vogen;

[assembly: VogenDefaults(swashbuckleSchemaFilterGeneration: SwashbuckleSchemaFilterGeneration.Generate)]


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    // opt.MapVogenTypes();
    // opt.MapType<CustomerName>(() => new OpenApiSchema { Type = "string" });
    opt.SchemaFilter<MyVogenSchemaFilter>();
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            {
                Centigrade temperatureC = Centigrade.From(Random.Shared.Next(-20, 55));
                return new WeatherForecast
                (
                    ForecastDate.From(DateOnly.FromDateTime(DateTime.Now.AddDays(index))),
                    temperatureC,
                    Farenheit.FromCentigrade(temperatureC), 
                    summaries[Random.Shared.Next(summaries.Length)],
                    City.From("London")
                );
            })
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/weatherforecast/{city}", (City city) =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            {
                Centigrade temperatureC = Centigrade.From(Random.Shared.Next(-20, 55));
                return new WeatherForecast
                (
                    ForecastDate.From(DateOnly.FromDateTime(DateTime.Now.AddDays(index))),
                    temperatureC,
                    Farenheit.FromCentigrade(temperatureC), 
                    summaries[Random.Shared.Next(summaries.Length)],
                    city
                );
            })
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecastByCity")
    .WithOpenApi();

app.Run();

record WeatherForecast(ForecastDate Date, Centigrade TemperatureC, Farenheit TemperatureF, string? Summary, City City)
{
}

[ValueObject<DateOnly>]
public partial struct ForecastDate
{}

[ValueObject<string>]
//[ValueObject<string>(parsableForStrings: ParsableForStrings.GenerateMethods)]
public partial struct City
{
}

[ValueObject<float>]
public partial struct Farenheit
{
    public static Farenheit FromCentigrade(Centigrade c) => From(32 + (int)(c.Value / 0.5556));
}

[ValueObject]
public partial struct Centigrade
{
}


public static class VogenSwashbuckleExtensions
{
    public static SwaggerGenOptions MapVogenTypes(this SwaggerGenOptions o)
    {
        SwaggerGenOptionsExtensions.MapType<CustomerName>(o, () => new OpenApiSchema { Type = "string" });
        o.MapType<OrderId>(() => new OpenApiSchema { Type = "integer" });
        o.MapType<Centigrade>(() => new OpenApiSchema { Type = "integer" });
        o.MapType<Farenheit>(() => new OpenApiSchema { Type = "number" });
        o.MapType<City>(() => new OpenApiSchema { Description = "The description of a City", Type = "string" });

        return o;
    }
}
