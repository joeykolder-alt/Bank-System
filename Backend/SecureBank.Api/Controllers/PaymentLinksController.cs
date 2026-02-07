using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;

namespace SecureBank.Api.Controllers;

[ApiController]
[Route("api/payment-links")]
[Authorize(Roles = "User")]
public class PaymentLinksController : ControllerBase
{
    private readonly IPaymentLinkService _paymentLinks;
    private readonly ITransactionService _transactions;
    private readonly ICurrentUserService _currentUser;

    public PaymentLinksController(
        IPaymentLinkService paymentLinks, 
        ITransactionService transactions,
        ICurrentUserService currentUser)
    {
        _paymentLinks = paymentLinks;
        _transactions = transactions;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Creates a new payment link for a merchant account
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentLinkDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaymentLinkDto>> Create(
        [FromBody] CreatePaymentLinkRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var result = await _paymentLinks.CreatePaymentLinkAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets a payment link by ID (public - anyone can view to make payment)
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaymentLinkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentLinkDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _paymentLinks.GetPaymentLinkAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets all payment links for the current user's merchant accounts
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentLinkDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PaymentLinkDto>>> GetMyPaymentLinks(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var result = await _paymentLinks.GetMerchantPaymentLinksAsync(userId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing payment link (only owner can update)
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PaymentLinkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentLinkDto>> Update(
        Guid id, 
        [FromBody] UpdatePaymentLinkRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var result = await _paymentLinks.UpdatePaymentLinkAsync(userId, id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Soft deletes a payment link (only owner can delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _paymentLinks.DeletePaymentLinkAsync(userId, id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Pays a payment link - creates a transaction from sender to merchant
    /// </summary>
    [HttpPost("pay")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Pay(
        [FromBody] PayPaymentLinkRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _paymentLinks.PayPaymentLinkAsync(userId, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Gets all transactions for a specific payment link (only owner can view)
    /// </summary>
    [HttpGet("{id:guid}/transactions")]
    [ProducesResponseType(typeof(PagedResultDto<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResultDto<TransactionDto>>> GetPaymentLinkTransactions(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var result = await _transactions.GetPaymentLinkTransactionsAsync(id, userId, page, pageSize, cancellationToken);
        return Ok(result);
    }
}
