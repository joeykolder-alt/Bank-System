using SecureBank.Application.DTOs;

namespace SecureBank.Application.Services;

public interface IKycService
{
    Task<KycApplicationCreateResult> SubmitApplicationAsync(string userId, string fullName, string email, string phoneNumber, string nationalIdNumber, int requestedPrimaryAccountType, Stream? nationalIdImage, Stream? residenceCardImage, string nationalIdFileName, string residenceCardFileName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KycApplicationListItemDto>> GetMyApplicationsAsync(string userId, CancellationToken cancellationToken = default);
}
