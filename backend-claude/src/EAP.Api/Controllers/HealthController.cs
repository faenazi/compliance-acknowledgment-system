using Microsoft.AspNetCore.Mvc;

namespace EAP.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("/health")]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new HealthResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }
}

public sealed record HealthResponse
{
    public string Status { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
}
