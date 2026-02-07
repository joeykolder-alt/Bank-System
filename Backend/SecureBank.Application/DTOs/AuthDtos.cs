namespace SecureBank.Application.DTOs;

public record RegisterRequest(string Email, string Password);
public record RegisterResponse(Guid UserId, string Email);

/// <summary>Response for POST /api/auth/register-with-kyc</summary>
public record RegisterWithKycResponse(string Status, string Message);

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, DateTime ExpiresAt, string Role);
