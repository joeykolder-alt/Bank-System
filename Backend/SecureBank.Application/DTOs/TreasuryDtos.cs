namespace SecureBank.Application.DTOs;

public record TreasuryBalanceDto(decimal Balance, string Currency);

public record TreasuryFundRequest(string ToIban, decimal Amount, string? Note);
