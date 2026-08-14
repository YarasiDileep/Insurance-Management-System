namespace Insurance.Api.DTOs;

public record PolicyDto(Guid Id, string PolicyNumber, Guid CustomerId, DateTime StartDate, DateTime EndDate, decimal Premium, string Status, DateTime CreatedAt);

public record CreatePolicyDto(string PolicyNumber, Guid CustomerId, DateTime StartDate, DateTime EndDate, decimal Premium);

public record UpdatePolicyDto(string PolicyNumber, DateTime StartDate, DateTime EndDate, decimal Premium, string Status);
