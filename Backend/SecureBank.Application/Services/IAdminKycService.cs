using SecureBank.Application.DTOs;

namespace SecureBank.Application.Services;

public interface IAdminKycService
{
    Task<PagedResultDto<KycApplicationListItemDto>> GetApplicationsAsync(string? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<KycApplicationDetailDto?> GetApplicationByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(byte[] Content, string ContentType)?> GetNationalIdImageAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<(byte[] Content, string ContentType)?> GetResidenceCardImageAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task ApproveAsync(Guid applicationId, string adminUserId, CancellationToken cancellationToken = default);
    Task RejectAsync(Guid applicationId, string adminUserId, string reason, CancellationToken cancellationToken = default);
}
