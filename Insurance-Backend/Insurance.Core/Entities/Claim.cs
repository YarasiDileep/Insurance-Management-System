namespace Insurance.Core.Entities;

public class Claim
{
    public Guid Id { get; set; }
    public string ClaimNumber { get; set; } = null!;
    public Guid PolicyId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime DateOfLoss { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
