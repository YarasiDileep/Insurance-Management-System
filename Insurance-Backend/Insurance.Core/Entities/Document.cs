namespace Insurance.Core.Entities;

public class Document
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? PolicyId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    // For simplicity we store a blob reference or path; in prod you'd use blob storage
    public string StoragePath { get; set; } = string.Empty;
}
