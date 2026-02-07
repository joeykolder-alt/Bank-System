namespace SecureBank.Infrastructure.Options;

public class JwtOptions
{
    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = "SecureBank";
    public string Audience { get; set; } = "SecureBank";
    public int ExpirationMinutes { get; set; } = 60;
}
