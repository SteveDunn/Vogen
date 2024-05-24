using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Vogen;

#if USE_SWASHBUCKLE
using Swashbuckle.AspNetCore.SwaggerGen;
#endif
#if USE_MICROSOFT_OPENAPI_AND_SCALAR
using Scalar.AspNetCore;
#endif

[assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod)]

var builder = WebApplication.CreateBuilder(args);

#if USE_MICROSOFT_OPENAPI_AND_SCALAR
    builder.Services.AddOpenApi((OpenApiOptions o) =>
    {
    });
#endif

#if USE_SWASHBUCKLE
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    // the following extension method is available if you specify `GenerateSwashbuckleMappingExtensionMethod` - as shown above
    opt.MapVogenTypes();
    
    // the following schema filter is generated if you specify GenerateSwashbuckleSchemaFilter as shown above
    // opt.SchemaFilter<MyVogenSchemaFilter>();
});
#endif

builder.Services.AddControllers();

var app = builder.Build();

#if USE_SWASHBUCKLE
    app.UseSwagger();
    app.UseSwaggerUI();
#endif


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

#if USE_MICROSOFT_OPENAPI_AND_SCALAR
app.MapOpenApi();
app.MapScalarApiReference();
#endif

app.Run();

