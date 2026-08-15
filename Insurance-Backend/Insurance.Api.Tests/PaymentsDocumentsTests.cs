using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Insurance.Infrastructure;
using Xunit;

namespace Insurance.Api.Tests;

public class PaymentsDocumentsTests
{
    private readonly WebApplicationFactory<Program> _factory;

    public PaymentsDocumentsTests()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            // UseSetting avoids relying on extension methods that may not be available
            builder.UseSetting(Microsoft.AspNetCore.Hosting.WebHostDefaults.EnvironmentKey, "Testing");
            builder.ConfigureServices(services =>
            {
                var toRemove = services.Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                                                  || d.ServiceType == typeof(ApplicationDbContext)).ToList();
                foreach (var d in toRemove) services.Remove(d);
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("InsuranceTestDb2"));
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
    public async Task Payment_Fails_When_Policy_Not_Belong_To_Customer()
    {
        var client = _factory.CreateClient();
        var agentToken = await GetTokenAsync(client, "agent");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", agentToken);

        // create a policy for customer A
        var policyPayload = new { PolicyNumber = "TST-1", CustomerId = Guid.NewGuid(), StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddYears(1), Premium = 100m };
        var polRes = await client.PostAsync("/api/Policies", new StringContent(JsonSerializer.Serialize(policyPayload), Encoding.UTF8, "application/json"));
        polRes.StatusCode.Should().Be(HttpStatusCode.Created);
        using var polDoc = JsonDocument.Parse(await polRes.Content.ReadAsStringAsync());
        var policyId = polDoc.RootElement.GetProperty("id").GetGuid();

        // attempt payment using different customer id
        var payPayload = new { PolicyId = policyId, CustomerId = Guid.NewGuid(), Amount = 50m, Method = "Card" };
        var payRes = await client.PostAsync("/api/Payments", new StringContent(JsonSerializer.Serialize(payPayload), Encoding.UTF8, "application/json"));
        payRes.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Document_Upload_Requires_Existing_Customer_And_Policy_Belongs()
    {
        var client = _factory.CreateClient();
        var agentToken = await GetTokenAsync(client, "agent");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", agentToken);

        // create customer
        var custPayload = new { FirstName = "T", LastName = "U", Email = "t@example.com", Phone = "+1" };
        var custRes = await client.PostAsync("/api/Customers", new StringContent(JsonSerializer.Serialize(custPayload), Encoding.UTF8, "application/json"));
        custRes.StatusCode.Should().Be(HttpStatusCode.Created);
        using var custDoc = JsonDocument.Parse(await custRes.Content.ReadAsStringAsync());
        var customerId = custDoc.RootElement.GetProperty("id").GetGuid();

        // create policy for that customer
        var policyPayload = new { PolicyNumber = "TST-2", CustomerId = customerId, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddYears(1), Premium = 100m };
        var polRes = await client.PostAsync("/api/Policies", new StringContent(JsonSerializer.Serialize(policyPayload), Encoding.UTF8, "application/json"));
        polRes.StatusCode.Should().Be(HttpStatusCode.Created);
        using var polDoc = JsonDocument.Parse(await polRes.Content.ReadAsStringAsync());
        var policyId = polDoc.RootElement.GetProperty("id").GetGuid();

        // attempt upload with mismatched policy/customer
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("hello"));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
        content.Add(fileContent, "file", "test.txt");
        content.Add(new StringContent(Guid.NewGuid().ToString()), "customerId");
        content.Add(new StringContent(policyId.ToString()), "policyId");

        var upRes = await client.PostAsync("/api/Documents", content);
        upRes.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
