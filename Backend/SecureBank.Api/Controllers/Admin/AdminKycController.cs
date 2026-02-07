using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;

namespace SecureBank.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/kyc/applications")]
[Authorize(Roles = "Admin")]
public class AdminKycController : ControllerBase
{
    private readonly IAdminKycService _adminKyc;
    private readonly ICurrentUserService _currentUser;

    public AdminKycController(IAdminKycService adminKyc, ICurrentUserService currentUser)
    {
        _adminKyc = adminKyc;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<KycApplicationListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<KycApplicationListItemDto>>> GetApplications(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _adminKyc.GetApplicationsAsync(status, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(KycApplicationDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KycApplicationDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var app = await _adminKyc.GetApplicationByIdAsync(id, cancellationToken);
        if (app == null) return NotFound();
        return Ok(app);
    }

    [HttpGet("{id:guid}/national-id-image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNationalIdImage(Guid id, CancellationToken cancellationToken)
    {
        var result = await _adminKyc.GetNationalIdImageAsync(id, cancellationToken);
        if (result == null) return NotFound();
        return File(result.Value.Content, result.Value.ContentType);
    }

    [HttpGet("{id:guid}/residence-card-image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetResidenceCardImage(Guid id, CancellationToken cancellationToken)
    {
        var result = await _adminKyc.GetResidenceCardImageAsync(id, cancellationToken);
        if (result == null) return NotFound();
        return File(result.Value.Content, result.Value.ContentType);
    }

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var adminUserId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _adminKyc.ApproveAsync(id, adminUserId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] KycRejectRequest request, CancellationToken cancellationToken)
    {
        var adminUserId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _adminKyc.RejectAsync(id, adminUserId, request.Reason, cancellationToken);
        return NoContent();
    }
}
