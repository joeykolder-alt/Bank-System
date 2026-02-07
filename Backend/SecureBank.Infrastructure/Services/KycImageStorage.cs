namespace SecureBank.Infrastructure.Services;

public interface IKycImageStorage
{
    Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default);
    Task<(byte[] Content, string ContentType)?> GetAsync(string path, CancellationToken cancellationToken = default);
}
