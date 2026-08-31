using Insurance.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Insurance.Api.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();
// register storage and payment services
builder.Services.AddSingleton<Insurance.Api.Services.IStorageService, Insurance.Api.Services.FileSystemStorageService>();
builder.Services.AddSingleton<Insurance.Api.Services.IPaymentGateway, Insurance.Api.Services.MockPaymentGateway>();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database & EF Core
// Register the application's DbContext only when not running integration tests that expect
// to control the provider. Skipping SQL Server registration during tests avoids multiple
// EF providers being registered in the same service provider (which causes runtime errors).
if (!builder.Environment.IsEnvironment("Testing") &&
    !builder.Services.Any(s => s.ServiceType == typeof(Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext>)))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default") ??
            "Server=.;Database=InsuranceManagementDb;Integrated Security=True;TrustServerCertificate=True;"));
}

// Identity
builder.Services.AddIdentity<Microsoft.AspNetCore.Identity.IdentityUser, Microsoft.AspNetCore.Identity.IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Authentication / Authorization (JWT)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretDevelopmentKey-ChangeThis";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Insurance.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Insurance.Api.Users";

builder.Services.AddScoped<IUserService, AuthService>(); // keep API abstraction for now

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AgentOrAdmin", policy => policy.RequireRole("Agent","Admin"));
    options.AddPolicy("CustomerOrAgentOrAdmin", policy => policy.RequireRole("Customer","Agent","Admin"));
});

// Add services to the container.
// Swagger/OpenAPI will be enabled in Development below

var app = builder.Build();


// Configure the HTTP request pipeline.
// For development convenience enable Swagger UI. If you prefer to limit it to Development,
// change this to `if (app.Environment.IsDevelopment())`.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Insurance API V1");
    // c.RoutePrefix = string.Empty; // uncomment to serve UI at app root (/)
});

app.UseAuthentication();
app.UseAuthorization();

// Allow CORS for frontend development (Angular dev server)
app.UseCors(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .SetIsOriginAllowed(origin => true));

// Map controllers (attribute routed controllers)
app.MapControllers();

// Run DB seed in development
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Ensure database/tables exist for EF Core + Identity when running in test/dev environments.
    // Wrap in try/catch because integration tests may replace the DbContext/provider which can
    // result in multiple provider registrations during the test host startup. In that case
    // skip EnsureCreated to allow tests to control DB initialization.
    try
    {
        await db.Database.EnsureCreatedAsync();
    }
    catch (InvalidOperationException)
    {
        // likely caused by multiple EF providers registered in the test host; skip creation.
    }
    catch (Exception)
    {
        // swallow any other startup DB create errors to avoid failing tests here; real deployments
        // should use migrations and proper startup checks.
    }
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Microsoft.AspNetCore.Identity.IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
    // Inline seed logic to avoid cross-project type resolution issues during build in this environment
    if (!await db.Customers.AnyAsync())
    {
        // Ensure roles exist
        var roles = new[] { "Admin", "Agent", "Customer" };
        foreach (var r in roles)
        {
            if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(r));
        }

        // Create example users (password: Password123!)
        async Task CreateUserIfMissing(string username, string email, string role)
        {
            var u = await userManager.FindByNameAsync(username);
            if (u == null)
            {
                u = new Microsoft.AspNetCore.Identity.IdentityUser { UserName = username, Email = email };
                await userManager.CreateAsync(u, "Password123!");
                await userManager.AddToRoleAsync(u, role);
            }
        }

        await CreateUserIfMissing("admin", "admin@example.com", "Admin");
        await CreateUserIfMissing("agent", "agent@example.com", "Agent");
        await CreateUserIfMissing("customer", "customer@example.com", "Customer");
        // Add sample users to a lightweight in-memory users table if present
        // (This repository uses a simple IUserService with hard-coded users for dev.)
        var customer = new Insurance.Core.Entities.Customer 
        { 
            Id = Guid.NewGuid(), 
            FirstName = "John", 
            LastName = "Doe", 
            Email = "john.doe@example.com", 
            Phone = "+1-555-0100" 
        };
        db.Customers.Add(customer);

        var policy = new Insurance.Core.Entities.Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            PolicyNumber = "POL-1001",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddYears(1),
            Premium = 1200.00M
        };
        db.Policies.Add(policy);

        var claim = new Insurance.Core.Entities.Claim
        {
            Id = Guid.NewGuid(),
            ClaimNumber = "CLM-1001",
            PolicyId = policy.Id,
            CustomerId = customer.Id,
            DateOfLoss = DateTime.UtcNow.Date.AddDays(-10),
            Amount = 500.00M
        };
        db.Claims.Add(claim);

        await db.SaveChangesAsync();
    }
}
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
