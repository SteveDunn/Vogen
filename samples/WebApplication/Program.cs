#pragma warning disable ASPDEPR002

using Microsoft.OpenApi;
using Vogen;
using WebApplication.Shared;

#if USE_SWASHBUCKLE
    [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod)]
#endif
#if USE_MICROSOFT_OPENAPI_AND_SCALAR
    using Microsoft.AspNetCore.OpenApi;
    using Scalar.AspNetCore;
    [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateOpenApiMappingExtensionMethod)]
#endif



var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

#if USE_MICROSOFT_OPENAPI_AND_SCALAR
    builder.Services.AddOpenApi((OpenApiOptions o) =>
    {
        o.MapVogenTypesInOpenApiMarkers();
    });
#endif


#if USE_SWASHBUCKLE
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    // the following extension method is available if you specify `GenerateSwashbuckleMappingExtensionMethod` - as shown above
    
    opt.MapVogenTypesInWebApplication();
    opt.MapVogenTypesInWebApplication_Shared();
    
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

app.MapGet("/historicweatherforecast/{historicForecastId}", (HistoricForecastId historicForecastId) =>
    {
        
            Centigrade temperatureC = Centigrade.From(42);
            WeatherForecast forecast = new(
                ForecastDate.From(new DateOnly(1970, 6, 10)),
                temperatureC,
                Farenheit.FromCentigrade(temperatureC),
                summaries[0] + " - related to historic forecast with an ID of " + historicForecastId,
                City.From("London"));
            
        return forecast;
    })
    .WithName("GetHistoricForecast")
    .WithOpenApi(generatedOperation =>
    {
        var parameter = generatedOperation.Parameters?[0];
        parameter?.Description = "The ID of the historical weather report (example only - always returns the same weather report)";
        parameter?.Examples?.Add("example1", new OpenApiExample { Value = Guid.NewGuid().ToString() });
        return generatedOperation;        
    });

#if USE_MICROSOFT_OPENAPI_AND_SCALAR
app.MapOpenApi();
app.MapScalarApiReference();
#endif

app.Run();

[OpenApiMarker<City>]
[OpenApiMarker<Age>]
[OpenApiMarker<Name>]
public partial class OpenApiMarkers;
