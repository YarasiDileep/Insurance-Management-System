namespace Insurance.Core.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public string Method { get; set; } = "Unknown";
    public string Reference { get; set; } = string.Empty;
}
