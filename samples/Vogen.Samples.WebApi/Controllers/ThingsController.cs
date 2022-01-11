using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Vogen.Samples.WebApi.Controllers;

[ValueObject(typeof(int), conversions: Conversions.TypeConverter | Conversions.SystemTextJson)]
public partial struct CustomerId
{
}

[ValueObject(typeof(int), conversions: Conversions.TypeConverter | Conversions.SystemTextJson)]
public partial struct SpaceshipId
{
}

[ValueObject(typeof(float), conversions: Conversions.TypeConverter | Conversions.SystemTextJson)]
public partial struct Velocity
{
}

public record CustomerDetails(CustomerId Id, string Name, string Address);

[ApiController]
[Route("[controller]")]
public class ThingsController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public ThingsController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet("customer/{id}")]
    public CustomerDetails Get(CustomerId id) => new(id, "Fred Flintstone", "wherever");

    [HttpGet("velocity/{spaceshipId}")]
    public Velocity Get(SpaceshipId id) => Velocity.From(.123f*id.Value);
}