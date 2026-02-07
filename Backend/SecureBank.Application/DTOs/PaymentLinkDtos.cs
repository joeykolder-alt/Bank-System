namespace SecureBank.Application.DTOs;

/// <summary>
/// Payment Link DTO - represents a payment link for merchants
/// </summary>
public record PaymentLinkDto(
    Guid Id,
    decimal Amount,
    string Currency,
    Guid MerchantId,
    string? MerchantIban,
    string ProductName,
    string? ProductDescription,
    string? ProductImageUrl,
    DateTime CreatedAt
);

/// <summary>
/// Request to create a new payment link
/// </summary>
public record CreatePaymentLinkRequest(
    string MerchantIban,
    decimal Amount,
    string Currency,
    string ProductName,
    string? ProductDescription,
    string? ProductImageUrl
);

/// <summary>
/// Request to update an existing payment link
/// </summary>
public record UpdatePaymentLinkRequest(
    decimal? Amount,
    string? Currency,
    string? ProductName,
    string? ProductDescription,
    string? ProductImageUrl
);

/// <summary>
/// Request to pay a payment link
/// </summary>
public record PayPaymentLinkRequest(
    Guid PaymentLinkId,
    string SenderIban
);
