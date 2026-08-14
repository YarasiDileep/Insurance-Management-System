namespace Insurance.Core.Entities;

public class Policy
{
    public Guid Id { get; set; }
    public string PolicyNumber { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Premium { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
