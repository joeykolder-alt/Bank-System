using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;

namespace SecureBank.Api.Controllers;

[ApiController]
[Route("api/accounts")]
[Authorize(Roles = "User")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accounts;
    private readonly ICurrentUserService _currentUser;

    public AccountsController(IAccountService accounts, ICurrentUserService currentUser)
    {
        _accounts = accounts;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BankAccountDto>>> GetAccounts(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var list = await _accounts.GetUserAccountsAsync(userId, cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BankAccountDto>> GetAccount(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var account = await _accounts.GetAccountAsync(id, userId, cancellationToken);
        if (account == null) return StatusCode(403);
        return Ok(account);
    }

    [HttpPost("subaccounts")]
    [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BankAccountDto>> CreateSubAccount([FromBody] CreateSubAccountRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var account = await _accounts.CreateSubAccountAsync(userId, request, cancellationToken);
        if (account == null) return BadRequest();
        return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
    }
}
