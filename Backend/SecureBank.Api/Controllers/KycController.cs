using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;

namespace SecureBank.Api.Controllers;

[ApiController]
[Route("api/kyc")]
[Authorize(Roles = "User")]
public class KycController : ControllerBase
{
    private readonly IKycService _kyc;
    private readonly ICurrentUserService _currentUser;

    public KycController(IKycService kyc, ICurrentUserService currentUser)
    {
        _kyc = kyc;
        _currentUser = currentUser;
    }

    [HttpPost("applications")]
    [ProducesResponseType(typeof(KycApplicationCreateResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<KycApplicationCreateResult>> SubmitApplication(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var form = await Request.ReadFormAsync(cancellationToken);
        var fullName = form["fullName"].ToString();
        var email = form["email"].ToString();
        var phoneNumber = form["phoneNumber"].ToString();
        var nationalIdNumber = form["nationalIdNumber"].ToString();
        var requestedPrimaryAccountType = int.Parse(form["requestedPrimaryAccountType"].ToString());
        var nationalIdImage = form.Files.GetFile("nationalIdImage");
        var residenceCardImage = form.Files.GetFile("residenceCardImage");

        Stream? natStream = nationalIdImage?.OpenReadStream();
        Stream? resStream = residenceCardImage?.OpenReadStream();
        try
        {
            var result = await _kyc.SubmitApplicationAsync(
                userId, fullName!, email!, phoneNumber!, nationalIdNumber!,
                requestedPrimaryAccountType,
                natStream, resStream,
                nationalIdImage?.FileName ?? "", residenceCardImage?.FileName ?? "",
                cancellationToken);
            return Created($"/api/kyc/applications/{result.ApplicationId}", result);
        }
        finally
        {
            natStream?.Dispose();
            resStream?.Dispose();
        }
    }

    [HttpGet("applications/me")]
    [ProducesResponseType(typeof(IReadOnlyList<KycApplicationListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<KycApplicationListItemDto>>> GetMyApplications(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var list = await _kyc.GetMyApplicationsAsync(userId, cancellationToken);
        return Ok(list);
    }
}
