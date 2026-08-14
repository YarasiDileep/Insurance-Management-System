namespace Insurance.Api.DTOs;

public record ClaimDto(Guid Id, string ClaimNumber, Guid PolicyId, Guid CustomerId, DateTime DateOfLoss, decimal Amount, string Status, DateTime CreatedAt);

public record CreateClaimDto(string ClaimNumber, Guid PolicyId, Guid CustomerId, DateTime DateOfLoss, decimal Amount);

public record UpdateClaimDto(string ClaimNumber, DateTime DateOfLoss, decimal Amount, string Status);
