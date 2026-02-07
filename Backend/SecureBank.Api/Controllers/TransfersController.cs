using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;

namespace SecureBank.Api.Controllers;

[ApiController]
[Route("api/transfers")]
[Authorize(Roles = "User")]
public class TransfersController : ControllerBase
{
    private readonly IMoneyService _money;
    private readonly ITransactionService _transactions;
    private readonly ICurrentUserService _currentUser;

    public TransfersController(IMoneyService money, ITransactionService transactions, ICurrentUserService currentUser)
    {
        _money = money;
        _transactions = transactions;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Deposit funds to an account
    /// </summary>
    [HttpPost("deposit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _money.DepositAsync(userId, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Withdraw funds from an account
    /// </summary>
    [HttpPost("withdraw")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _money.WithdrawAsync(userId, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Transfer funds between accounts
    /// </summary>
    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _money.TransferAsync(userId, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get transaction history for an account by ID
    /// </summary>
    [HttpGet("account/{accountId:guid}")]
    [ProducesResponseType(typeof(PagedResultDto<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResultDto<TransactionDto>>> GetAccountTransactions(
        Guid accountId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _transactions.GetAccountTransactionsAsync(accountId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get transaction history for an account by IBAN
    /// </summary>
    [HttpGet("iban/{iban}")]
    [ProducesResponseType(typeof(PagedResultDto<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResultDto<TransactionDto>>> GetAccountTransactionsByIban(
        string iban,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _transactions.GetAccountTransactionsByIbanAsync(iban, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a single transaction by ID
    /// </summary>
    [HttpGet("{transactionId:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionDto>> GetTransactionById(
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        var result = await _transactions.GetTransactionByIdAsync(transactionId, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
