using Microsoft.AspNetCore.Mvc;

namespace ServerApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Simple ping endpoint for basic connectivity check
    /// </summary>
    [HttpGet("ping")]
    public ActionResult Ping() => Ok("pong");

    /// <summary>
    /// Health check endpoint for Docker and load balancer health monitoring
    /// </summary>
    [HttpGet("health")]
    public ActionResult Health() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}