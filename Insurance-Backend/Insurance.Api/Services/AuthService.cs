using System.Threading.Tasks;

namespace Insurance.Api.Services;

// Simple development authentication service.
// In a production app replace this with IdentityServer/ASP.NET Identity or an external provider.
public class AuthService : IUserService
{
    private readonly List<UserInfo> _users = new()
    {
        // Admin user (full privileges)
        new UserInfo(Guid.Parse("00000000-0000-0000-0000-000000000001"), "admin", new[] { "Admin" }, "admin@example.com"),
        // Customer user (can file/view own claims; limited in this sample)
        new UserInfo(Guid.Parse("00000000-0000-0000-0000-000000000002"), "customer", new[] { "Customer" }, "customer@example.com"),
        // Agent user (can manage policies/claims)
        new UserInfo(Guid.Parse("00000000-0000-0000-0000-000000000003"), "agent", new[] { "Agent" }, "agent@example.com")
    };

    public Task<UserInfo?> ValidateCredentialsAsync(string username, string password)
    {
        // Development-only: accept password == "Password123!" for both users
        if (string.IsNullOrWhiteSpace(username) || password != "Password123!")
            return Task.FromResult<UserInfo?>(null);

        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }
}
