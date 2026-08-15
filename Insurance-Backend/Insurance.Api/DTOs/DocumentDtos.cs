namespace Insurance.Api.DTOs;

public record DocumentDto(Guid Id, Guid CustomerId, Guid? PolicyId, string FileName, string ContentType, long Size, DateTime UploadedAt, string StoragePath);

public record CreateDocumentDto(Guid CustomerId, Guid? PolicyId, string FileName, string ContentType, long Size, string StoragePath);
