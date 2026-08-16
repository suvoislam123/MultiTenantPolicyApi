// PolicyApi.Tests/PolicyEndpointTests.cs
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MultiTenantPolicyApi.Tests;

public class PolicyEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Guid _tenantA = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _tenantB = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly Guid _customerB = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private readonly Guid _customerA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public PolicyEndpointTests(WebApplicationFactory<Program> factory) => _factory = factory;

    private async Task<HttpClient> ClientForTenantAsync(Guid tenantId)
    {
        var client = _factory.CreateClient();
        var tokenResp = await client.GetFromJsonAsync<TokenResponse>($"/dev/token?tenantId={tenantId}");
        client.DefaultRequestHeaders.Authorization = new("Bearer", tokenResp!.token);
        return client;
    }

    [Fact]
    public async Task TenantA_Cannot_Access_TenantB_Customer_Policy()
    {
        var client = await ClientForTenantAsync(_tenantA);

        var response = await client.GetAsync($"/api/customers/{_customerB}/policy");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task TenantA_Can_Access_Own_Customer_Policy()
    {
        var client = await ClientForTenantAsync(_tenantA);

        var response = await client.GetAsync($"/api/customers/{_customerA}/policy");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Expiring_Policies_Only_Returns_Own_Tenant()
    {
        var client = await ClientForTenantAsync(_tenantB);

        var policies = await client.GetFromJsonAsync<List<PolicyDto>>("/api/policies/expiring?withinDays=365");

        Assert.All(policies!, p => Assert.NotEqual(_customerA, p.CustomerId));
    }

    [Fact]
    public async Task Unauthenticated_Request_Is_Rejected()
    {
        var client = _factory.CreateClient(); // no token attached

        var response = await client.GetAsync($"/api/customers/{_customerA}/policy");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private record TokenResponse(string token);
    private record PolicyDto(Guid Id, Guid CustomerId, string PolicyNumber, DateTime ExpirationDate, decimal PremiumAmount);
}
