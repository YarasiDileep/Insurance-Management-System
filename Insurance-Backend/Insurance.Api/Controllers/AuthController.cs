using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Insurance.Api.Services;

namespace Insurance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _users;
    private readonly IConfiguration _config;

    public AuthController(IUserService users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] LoginRequest req)
    {
        var user = await _users.ValidateCredentialsAsync(req.Username, req.Password);
        if (user == null) return Unauthorized();

        var jwtKey = _config["Jwt:Key"] ?? "SuperSecretDevelopmentKey-ChangeThis";
        var jwtIssuer = _config["Jwt:Issuer"] ?? "Insurance.Api";
        var jwtAudience = _config["Jwt:Audience"] ?? "Insurance.Api.Users";

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id.ToString())
        };
        foreach (var role in user.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}

public record LoginRequest(string Username, string Password);
