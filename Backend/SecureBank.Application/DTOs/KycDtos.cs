using SecureBank.Domain.Enums;

namespace SecureBank.Application.DTOs;

public record CustomerProfileDto(
    Guid Id,
    string UserId,
    string FullName,
    string Email,
    string Phone,
    string? NationalIdNumber,
    CustomerProfileStatus Status,
    DateTime CreatedAt,
    BankAccountSummaryDto? PrimaryAccount
);

public record KycApplicationCreateResult(Guid ApplicationId, KycStatus Status);

public record KycApplicationListItemDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string NationalIdNumber,
    int RequestedPrimaryAccountType,
    KycStatus Status,
    DateTime SubmittedAt,
    DateTime? ReviewedAt,
    string? RejectionReason
);

public record KycApplicationDetailDto(
    Guid Id,
    string UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    string NationalIdNumber,
    int RequestedPrimaryAccountType,
    KycStatus Status,
    DateTime SubmittedAt,
    DateTime? ReviewedAt,
    string? ReviewedByAdminId,
    string? RejectionReason
);

public record KycRejectRequest(string Reason);
