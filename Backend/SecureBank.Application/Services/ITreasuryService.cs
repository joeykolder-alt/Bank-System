using SecureBank.Application.DTOs;

namespace SecureBank.Application.Services;

public interface ITreasuryService
{
    Task<TreasuryBalanceDto> GetBalanceAsync(CancellationToken cancellationToken = default);
    Task FundAccountAsync(string toIban, decimal amount, string? note, string adminUserId, CancellationToken cancellationToken = default);
}
