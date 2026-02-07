using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;

namespace SecureBank.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/payroll")]
[Authorize(Roles = "Admin")]
public class AdminPayrollController : ControllerBase
{
    private readonly IPayrollService _payroll;

    public AdminPayrollController(IPayrollService payroll)
    {
        _payroll = payroll;
    }

    [HttpPost("assign")]
    [ProducesResponseType(typeof(PayrollProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayrollProfileDto>> Assign([FromBody] AssignPayrollRequest request, CancellationToken cancellationToken)
    {
        var result = await _payroll.AssignPayrollAsync(request, cancellationToken);
        return Created("/api/admin/payroll", result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PayrollProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PayrollProfileDto>>> GetPayrolls(CancellationToken cancellationToken)
    {
        var list = await _payroll.GetPayrollProfilesAsync(cancellationToken);
        return Ok(list);
    }
}
