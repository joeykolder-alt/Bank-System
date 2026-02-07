using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureBank.Infrastructure.Persistence;

namespace SecureBank.Api.Controllers.Dev;

/// <summary>
/// Development-only endpoint to verify KYC persistence. Returns 404 when not in Development.
/// </summary>
[ApiController]
[Route("api/dev/kyc")]
[AllowAnonymous]
public class DevKycController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public DevKycController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    /// <summary>
    /// Returns KYC application counts by status. Development only.
    /// </summary>
    [HttpGet("count")]
    [ProducesResponseType(typeof(KycCountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KycCountResponse>> GetCount(CancellationToken cancellationToken)
    {
        if (!_env.IsDevelopment())
            return NotFound();

        // KycStatus: Pending=0, Approved=1, Rejected=2 (SecureBank.Domain.Enums.KycStatus)
        var pending = await _db.KycApplications.CountAsync(a => (int)a.Status == 0, cancellationToken);
        var approved = await _db.KycApplications.CountAsync(a => (int)a.Status == 1, cancellationToken);
        var rejected = await _db.KycApplications.CountAsync(a => (int)a.Status == 2, cancellationToken);
        return Ok(new KycCountResponse(pending, approved, rejected));
    }

    public record KycCountResponse(int Pending, int Approved, int Rejected);
}
