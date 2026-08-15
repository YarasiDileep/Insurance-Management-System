using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Insurance.Api.Services;

// Adapter service so parts of the app (and tests) can rely on the existing IUserService
// while we migrate to ASP.NET Identity. In time you can remove this and use UserManager directly.
public class AuthService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;

    public AuthService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserInfo?> ValidateCredentialsAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return null;
        var ok = await _userManager.CheckPasswordAsync(user, password);
        if (!ok) return null;

        var roles = await _userManager.GetRolesAsync(user);
        // Map IdentityUser to the lightweight UserInfo used by the API
        return new UserInfo(Guid.TryParse(user.Id, out var id) ? id : Guid.NewGuid(), user.UserName ?? username, roles.ToArray(), user.Email ?? string.Empty);
    }
}
