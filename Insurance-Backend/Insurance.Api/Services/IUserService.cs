using Insurance.Core.Entities;

namespace Insurance.Api.Services;

public interface IUserService
{
    // Validate a user and return a JWT-compatible user model
    Task<UserInfo?> ValidateCredentialsAsync(string username, string password);
}

public record UserInfo(Guid Id, string Username, string[] Roles, string Email);
