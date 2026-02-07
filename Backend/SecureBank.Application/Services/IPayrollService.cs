using SecureBank.Application.DTOs;

namespace SecureBank.Application.Services;

public interface IPayrollService
{
    Task<PayrollProfileDto> AssignPayrollAsync(AssignPayrollRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PayrollProfileDto>> GetPayrollProfilesAsync(CancellationToken cancellationToken = default);
}
