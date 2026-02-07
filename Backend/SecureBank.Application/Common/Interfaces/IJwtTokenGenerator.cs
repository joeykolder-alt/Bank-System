namespace SecureBank.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) Generate(string userId, string email, IList<string> roles);
}
