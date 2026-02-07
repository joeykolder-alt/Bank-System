using SecureBank.Application.DTOs;

namespace SecureBank.Application.Services;

public interface IProfileService
{
    Task<CustomerProfileDto?> GetMyProfileAsync(string userId, CancellationToken cancellationToken = default);
}
