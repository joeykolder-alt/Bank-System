using SecureBank.Application.DTOs;

namespace SecureBank.Application.Services;

public interface IPaymentLinkService
{
    /// <summary>
    /// Creates a new payment link for a merchant account
    /// </summary>
    Task<PaymentLinkDto> CreatePaymentLinkAsync(string userId, CreatePaymentLinkRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a payment link by ID (public access for customers)
    /// </summary>
    Task<PaymentLinkDto> GetPaymentLinkAsync(Guid paymentLinkId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all payment links for a merchant (by user ID)
    /// </summary>
    Task<IReadOnlyList<PaymentLinkDto>> GetMerchantPaymentLinksAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates a payment link (owner only)
    /// </summary>
    Task<PaymentLinkDto> UpdatePaymentLinkAsync(string userId, Guid paymentLinkId, UpdatePaymentLinkRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Soft deletes a payment link (owner only)
    /// </summary>
    Task DeletePaymentLinkAsync(string userId, Guid paymentLinkId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Pays a payment link - creates a transaction from sender to merchant
    /// </summary>
    Task PayPaymentLinkAsync(string userId, PayPaymentLinkRequest request, CancellationToken cancellationToken = default);
}
