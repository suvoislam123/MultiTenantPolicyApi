using MultiTenantPolicyApi.Auth;
using MultiTenantPolicyApi.Data;
using Microsoft.EntityFrameworkCore;


namespace MultiTenantPolicyApi.Endpoints;

public static  class PolicyEndpoints
{
    public static void MapPolicyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .RequireAuthorization();
        group.MapGet("customers/{customerId:guid}/policy", GetCustomerPolicy)
            .WithSummary("Get policy for a customer — tenant-scoped; cross-tenant access returns 404");
        group.MapGet("/policies/expiring", GetExpiringPolicies)
            .WithSummary("List policies expiring within N days, scoped to the caller's tenant");

        group.MapGet("/policies/{policyId:guid}/expiration", GetPolicyExpiration)
            .WithSummary("Deterministic lookup — exact stored expiration date or 404, never inferred");
    }
    private static async Task<IResult> GetCustomerPolicy(
        Guid customerId, AppDbContext db, ITenantContext tenantCtx)
    {
        // Tenant filter comes from the JWT claim, never from the route — customerId alone is never trusted.
        var policy = await db.Policies
            .Join(db.Customers, p => p.CustomerId, c => c.Id, (p, c) => new { p, c })
            .Where(x => x.c.Id == customerId && x.c.TenantId == tenantCtx.TenantId)
            .Select(x => x.p)
            .FirstOrDefaultAsync();

        return policy is null
            ? Results.NotFound() // 404 not 403 — don't confirm the customer exists in another tenant
            : Results.Ok(policy);
    }

    private static async Task<IResult> GetExpiringPolicies(
        int withinDays, AppDbContext db, ITenantContext tenantCtx)
    {
        var cutoff = DateTime.UtcNow.AddDays(withinDays);

        var policies = await db.Policies
            .Join(db.Customers, p => p.CustomerId, c => c.Id, (p, c) => new { p, c })
            .Where(x => x.c.TenantId == tenantCtx.TenantId && x.p.ExpirationDate <= cutoff)
            .Select(x => x.p)
            .ToListAsync();

        return Results.Ok(policies);
    }

    private static async Task<IResult> GetPolicyExpiration(
        Guid policyId, AppDbContext db, ITenantContext tenantCtx)
    {
        var expiration = await db.Policies
            .Join(db.Customers, p => p.CustomerId, c => c.Id, (p, c) => new { p, c })
            .Where(x => x.p.Id == policyId && x.c.TenantId == tenantCtx.TenantId)
            .Select(x => (DateTime?)x.p.ExpirationDate)
            .FirstOrDefaultAsync();

        return expiration is null
            ? Results.NotFound(new { message = "No policy found for this ID in your tenant." })
            : Results.Ok(new { policyId, expirationDate = expiration });
    }
}
