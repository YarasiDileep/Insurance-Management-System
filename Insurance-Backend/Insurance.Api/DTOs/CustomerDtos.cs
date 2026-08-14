namespace Insurance.Api.DTOs;

public record CustomerDto(Guid Id, string FirstName, string LastName, string Email, string Phone, DateTime DateOfBirth, DateTime CreatedAt);

public record CreateCustomerDto(string FirstName, string LastName, string Email, string Phone, DateTime DateOfBirth);

public record UpdateCustomerDto(string FirstName, string LastName, string Email, string Phone, DateTime DateOfBirth);
