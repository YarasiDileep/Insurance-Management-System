using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Insurance.Infrastructure;
using Xunit;

namespace Insurance.Api.Tests;

public class RbacTests
{
    private readonly WebApplicationFactory<Program> _factory;

    public RbacTests()
    {
        // Use an in-memory EF Core database for tests so Identity and application data are isolated.
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            // Signal to the app that tests are running so the app can avoid registering
            // the SQL Server provider at startup (tests will replace the DbContext with InMemory).
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove existing ApplicationDbContext / DbContextOptions registrations so we can replace them with InMemory.
                var toRemove = services.Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                                                  || d.ServiceType == typeof(ApplicationDbContext)
                                                  || d.ImplementationType == typeof(ApplicationDbContext)).ToList();
                foreach (var d in toRemove) services.Remove(d);

                // Add ApplicationDbContext using in-memory provider
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InsuranceTestDb");
                });
            });
        });
    }

    private async Task<string> GetTokenAsync(HttpClient client, string username)
    {
        var req = new { Username = username, Password = "Password123!" };
        var res = await client.PostAsync("/api/Auth/token", new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        return doc.RootElement.GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task Policies_Post_Requires_AgentOrAdmin()
    {
        var client = _factory.CreateClient();

        // anonymous should be unauthorized
        var anonResp = await client.PostAsync("/api/Policies", new StringContent(JsonSerializer.Serialize(new { PolicyNumber = "POL-9000", CustomerId = Guid.NewGuid(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1), Premium = 100m }), Encoding.UTF8, "application/json"));
        anonResp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // customer should be forbidden
        var custToken = await GetTokenAsync(client, "customer");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", custToken);
        var custResp = await client.PostAsync("/api/Policies", new StringContent(JsonSerializer.Serialize(new { PolicyNumber = "POL-9001", CustomerId = Guid.NewGuid(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1), Premium = 200m }), Encoding.UTF8, "application/json"));
        custResp.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // agent should be allowed
        var agentToken = await GetTokenAsync(client, "agent");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", agentToken);
        var agentResp = await client.PostAsync("/api/Policies", new StringContent(JsonSerializer.Serialize(new { PolicyNumber = "POL-9002", CustomerId = Guid.NewGuid(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1), Premium = 300m }), Encoding.UTF8, "application/json"));
        agentResp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Claims_Post_Allows_Customer_Agent_Admin()
    {
        var client = _factory.CreateClient();

        // Get tokens for each
        var custToken = await GetTokenAsync(client, "customer");
        var agentToken = await GetTokenAsync(client, "agent");
        var adminToken = await GetTokenAsync(client, "admin");

        var payload = new { ClaimNumber = "CLM-9000", PolicyId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), DateOfLoss = DateTime.UtcNow.AddDays(-1), Amount = 100m };

        // customer
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", custToken);
        var r1 = await client.PostAsync("/api/Claims", new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        r1.StatusCode.Should().Be(HttpStatusCode.Created);

        // agent
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", agentToken);
        var r2 = await client.PostAsync("/api/Claims", new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        r2.StatusCode.Should().Be(HttpStatusCode.Created);

        // admin
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var r3 = await client.PostAsync("/api/Claims", new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        r3.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
