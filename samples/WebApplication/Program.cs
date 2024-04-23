using Refit;
using Vogen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
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
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
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

record WeatherForecast(DateOnly Date, Centigrade TemperatureC, Farenheit TemperatureF, string? Summary, City City)
{
}

[ValueObject<string>]
//[ValueObject<string>(parsableForStrings: ParsableForStrings.GenerateMethods)]
public partial struct City
{
}

[ValueObject]
public partial struct Farenheit
{
    public static Farenheit FromCentigrade(Centigrade c) => From(32 + (int)(c.Value / 0.5556));
}

[ValueObject]
public partial struct Centigrade
{
}
