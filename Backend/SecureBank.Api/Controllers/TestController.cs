using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureBank.Api.Controllers;

/// <summary>
/// Temporary test endpoints for verification. Remove in production.
/// </summary>
[ApiController]
[Route("api/test")]
[AllowAnonymous]
public class TestController : ControllerBase
{
    /// <summary>
    /// Ping endpoint - returns 200 OK. Use to verify Swagger and unauthenticated access.
    /// </summary>
    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        return Ok(new { status = "OK", message = "pong" });
    }
}
