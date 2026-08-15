namespace Insurance.Api.Services;

public interface IStorageService
{
    // Saves a file stream and returns a storage path/reference
    Task<string> SaveFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);

    // Reads a file as stream by storage path
    Task<Stream?> GetFileAsync(string storagePath, CancellationToken cancellationToken = default);

    // Deletes a file
    Task<bool> DeleteFileAsync(string storagePath, CancellationToken cancellationToken = default);
}
