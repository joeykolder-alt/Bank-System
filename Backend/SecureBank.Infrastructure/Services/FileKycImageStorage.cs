using Microsoft.Extensions.Options;

namespace SecureBank.Infrastructure.Services;

public class FileKycImageStorage : IKycImageStorage
{
    private readonly string _basePath;

    public FileKycImageStorage(IOptions<Infrastructure.Options.KycStorageOptions> options)
    {
        _basePath = Path.GetFullPath(options.Value.BasePath);
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        var safeName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var fullPath = Path.Combine(_basePath, safeName);
        await using var fs = File.Create(fullPath);
        await content.CopyToAsync(fs, cancellationToken);
        return safeName;
    }

    public Task<(byte[] Content, string ContentType)?> GetAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, path);
        if (!File.Exists(fullPath)) return Task.FromResult<(byte[] Content, string ContentType)?>(null);
        var bytes = File.ReadAllBytes(fullPath);
        var ext = Path.GetExtension(path).ToLowerInvariant();
        var contentType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };
        return Task.FromResult<(byte[] Content, string ContentType)?>((bytes, contentType));
    }
}
