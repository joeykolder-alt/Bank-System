using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Api.Models;
using SecureBank.Application.DTOs;
using SecureBank.Application.Exceptions;
using SecureBank.Application.Services;

namespace SecureBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IKycService _kyc;

    public AuthController(IAuthService auth, IKycService kyc)
    {
        _auth = auth;
        _kyc = kyc;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _auth.RegisterAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Register), result);
    }

    /// <summary>
    /// Register and submit KYC in one step. Public endpoint (no auth). Accepts multipart/form-data with all fields and two image files.
    /// </summary>
    /// <remarks>
    /// Previous implementation failed because: (1) No [FromForm] parameter meant Swagger saw "No parameters" and could not generate a schema.
    /// (2) Reading Request.Form and Request.Form.Files inside the action runs after model binding; for multipart, the request body can only be read once,
    /// so if anything else (e.g. middleware or binding) touched the body, or when binding was skipped, Form was null or incomplete causing NullReferenceException at line 43.
    /// (3) Manual parsing and validation led to crashes instead of returning ValidationProblem. Using a strongly-typed [FromForm] DTO with DataAnnotations
    /// ensures the framework binds and validates the request, and returns 400 with ProblemDetails/ValidationProblem when invalid.
    /// </remarks>
    [HttpPost("register-with-kyc")]
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(RegisterWithKycResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterWithKycResponse>> RegisterWithKyc([FromForm] RegisterWithKycRequest request, CancellationToken cancellationToken)
    {
        // Model binding + DataAnnotations validation; return standard ValidationProblem so frontend gets clear field-level errors
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var email = request.Email.Trim();
        var password = request.Password;
        var fullName = request.FullName.Trim();
        var phoneNumber = request.PhoneNumber.Trim();
        var nationalIdNumber = request.NationalIdNumber.Trim();
        var nationalIdImage = request.NationalIdImage;
        var residenceCardImage = request.ResidenceCardImage;

        // Extra safety: required files are also validated by [Required]; if binding fails for files, ModelState would be invalid
        if (nationalIdImage == null || nationalIdImage.Length == 0)
        {
            ModelState.AddModelError(nameof(RegisterWithKycRequest.NationalIdImage), "National ID image is required.");
            return ValidationProblem(ModelState);
        }
        if (residenceCardImage == null || residenceCardImage.Length == 0)
        {
            ModelState.AddModelError(nameof(RegisterWithKycRequest.ResidenceCardImage), "Residence card image is required.");
            return ValidationProblem(ModelState);
        }

        RegisterResponse registerResult;
        try
        {
            registerResult = await _auth.RegisterAsync(new RegisterRequest(email, password), cancellationToken);
        }
        catch (ConflictException ex)
        {
            return Conflict(new ProblemDetails
            {
                Type = "https://httpstatuses.com/409",
                Title = "Conflict",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }

        await using (var natStream = nationalIdImage.OpenReadStream())
        await using (var resStream = residenceCardImage.OpenReadStream())
        {
            try
            {
                await _kyc.SubmitApplicationAsync(
                    registerResult.UserId.ToString(),
                    fullName, email, phoneNumber, nationalIdNumber,
                    request.RequestedPrimaryAccountType,
                    natStream, resStream,
                    nationalIdImage.FileName ?? "nationalId", residenceCardImage.FileName ?? "residenceCard",
                    cancellationToken);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("pending", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new ProblemDetails
                {
                    Type = "https://httpstatuses.com/409",
                    Title = "Conflict",
                    Detail = "You already have a pending request",
                    Status = StatusCodes.Status409Conflict
                });
            }
        }

        return CreatedAtAction(nameof(RegisterWithKyc), new RegisterWithKycResponse("Pending", "Your request is under review"));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _auth.LoginAsync(request, cancellationToken);
        if (result == null) return Unauthorized();
        return Ok(result);
    }
}
