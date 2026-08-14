using Insurance.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Insurance.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database & EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default") ??
        "Server=.;Database=InsuranceManagementDb;Integrated Security=True;TrustServerCertificate=True;"));

// Authentication / Authorization (JWT)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretDevelopmentKey-ChangeThis";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Insurance.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Insurance.Api.Users";

builder.Services.AddSingleton<IUserService, AuthService>();

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

// Map controllers (attribute routed controllers)
app.MapControllers();

// Run DB seed in development
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Inline seed logic to avoid cross-project type resolution issues during build in this environment
    if (!await db.Customers.AnyAsync())
    {
        var customer = new Insurance.Core.Entities.Customer { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Phone = "+1-555-0100" };
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
