using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;

namespace SecureBank.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/treasury")]
[Authorize(Roles = "Admin")]
public class AdminTreasuryController : ControllerBase
{
    private readonly ITreasuryService _treasury;
    private readonly ICurrentUserService _currentUser;

    public AdminTreasuryController(ITreasuryService treasury, ICurrentUserService currentUser)
    {
        _treasury = treasury;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(TreasuryBalanceDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TreasuryBalanceDto>> GetBalance(CancellationToken cancellationToken)
    {
        var result = await _treasury.GetBalanceAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("fund")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Fund([FromBody] TreasuryFundRequest request, CancellationToken cancellationToken)
    {
        var adminUserId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _treasury.FundAccountAsync(request.ToIban, request.Amount, request.Note, adminUserId, cancellationToken);
        return NoContent();
    }
}
