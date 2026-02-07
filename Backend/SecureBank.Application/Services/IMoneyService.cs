using SecureBank.Application.DTOs;

namespace SecureBank.Application.Services;

public interface IMoneyService
{
    Task DepositAsync(string userId, DepositRequest request, CancellationToken cancellationToken = default);
    Task WithdrawAsync(string userId, WithdrawRequest request, CancellationToken cancellationToken = default);
    Task TransferAsync(string userId, TransferRequest request, CancellationToken cancellationToken = default);
}
