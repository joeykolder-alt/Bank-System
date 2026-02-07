namespace SecureBank.Application.DTOs;

public record BankAccountDto(
    Guid Id,
    string IBAN,
    int AccountType,
    decimal Balance,
    string BalanceCurrency,
    bool IsPrimary,
    Guid? ParentAccountId,
    DateTime CreatedAt
);

public record BankAccountSummaryDto(
    Guid Id,
    string IBAN,
    int AccountType,
    decimal Balance,
    bool IsPrimary
);

public record CreateSubAccountRequest(
    Guid ParentAccountId,
    string AccountType,
    CardDetailsDto Card,
    decimal? InitialDeposit
);

public record CardDetailsDto(
    string CardHolderName,
    string CardNumber,
    int ExpMonth,
    int ExpYear,
    string BillingAddress
);

public record DepositRequest(string Iban, decimal Amount, string? Note);
public record WithdrawRequest(string Iban, decimal Amount, string? Note);

/// <summary>Transfer by IBAN only. fromIban must belong to the authenticated user.</summary>
public record TransferRequest(string FromIban, string ToIban, decimal Amount, string? Note);
