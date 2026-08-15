using System.IO;

namespace Insurance.Api.Services;

// Simple filesystem-based storage for demo purposes. Stores files under ./storage
public class FileSystemStorageService : IStorageService
{
    private readonly string _basePath;

    public FileSystemStorageService(IConfiguration config)
    {
        var p = config["Storage:BasePath"];
        _basePath = string.IsNullOrWhiteSpace(p) ? Path.Combine(AppContext.BaseDirectory, "storage") : p;
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid().ToString();
        var safe = Path.GetFileName(fileName);
        var path = Path.Combine(_basePath, id + "__" + safe);
        using var fs = File.Create(path);
        await stream.CopyToAsync(fs, cancellationToken);
        return path;
    }

    public Task<Stream?> GetFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(storagePath)) return Task.FromResult<Stream?>(null);
        Stream s = File.OpenRead(storagePath);
        return Task.FromResult<Stream?>(s);
    }

    public Task<bool> DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(storagePath)) return Task.FromResult(false);
        File.Delete(storagePath);
        return Task.FromResult(true);
    }
}
