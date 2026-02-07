using SecureBank.Application.DTOs;

namespace SecureBank.Application.Services;

public interface IAccountService
{
    /// <summary>
    /// Gets all accounts for a user
    /// </summary>
    Task<IReadOnlyList<BankAccountDto>> GetUserAccountsAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific account by ID (only if owned by user)
    /// </summary>
    Task<BankAccountDto?> GetAccountAsync(Guid accountId, string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a sub-account under a parent account
    /// </summary>
    Task<BankAccountDto?> CreateSubAccountAsync(string userId, CreateSubAccountRequest request, CancellationToken cancellationToken = default);
}
