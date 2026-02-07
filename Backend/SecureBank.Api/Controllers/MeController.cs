using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;

namespace SecureBank.Api.Controllers;

[ApiController]
[Route("api/me")]
[Authorize(Roles = "User")]
public class MeController : ControllerBase
{
    private readonly IProfileService _profile;
    private readonly ICurrentUserService _currentUser;

    public MeController(IProfileService profile, ICurrentUserService currentUser)
    {
        _profile = profile;
        _currentUser = currentUser;
    }

    [HttpGet("profile")]
    [ProducesResponseType(typeof(CustomerProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var profile = await _profile.GetMyProfileAsync(userId, cancellationToken);
        if (profile == null) return NotFound();
        return Ok(profile);
    }
}
