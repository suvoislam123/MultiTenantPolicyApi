using Microsoft.EntityFrameworkCore;
using MultiTenantPolicyApi.Models;

namespace MultiTenantPolicyApi.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        // Don't seed again if data already exists
        if (await db.Tenants.AnyAsync())
            return;

        // =========================
        // Tenants
        // =========================

        var tenant1 = new Tenant
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Acme Corp"
        };

        var tenant2 = new Tenant
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Globex Inc"
        };

        // =========================
        // Customers
        // =========================

        var customer1 = new Customer
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            TenantId = tenant1.Id,
            Name = "John Doe"
        };

        var customer2 = new Customer
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            TenantId = tenant1.Id,
            Name = "Jane Smith"
        };

        var customer3 = new Customer
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            TenantId = tenant2.Id,
            Name = "Michael Johnson"
        };

        var customer4 = new Customer
        {
            Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            TenantId = tenant2.Id,
            Name = "Sarah Williams"
        };

        // =========================
        // Policies
        // =========================

        var policy1 = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = customer1.Id,
            PolicyNumber = "ACME-POL-001",
            ExpirationDate = DateTime.UtcNow.Date.AddDays(10),
            PremiumAmount = 1250.00m
        };

        var policy2 = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = customer1.Id,
            PolicyNumber = "ACME-POL-002",
            ExpirationDate = DateTime.UtcNow.Date.AddDays(90),
            PremiumAmount = 2500.00m
        };

        var policy3 = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = customer2.Id,
            PolicyNumber = "ACME-POL-003",
            ExpirationDate = DateTime.UtcNow.Date.AddDays(25),
            PremiumAmount = 1800.00m
        };

        var policy4 = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = customer3.Id,
            PolicyNumber = "GLOBEX-POL-001",
            ExpirationDate = DateTime.UtcNow.Date.AddDays(15),
            PremiumAmount = 3200.00m
        };

        var policy5 = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = customer3.Id,
            PolicyNumber = "GLOBEX-POL-002",
            ExpirationDate = DateTime.UtcNow.Date.AddDays(120),
            PremiumAmount = 4500.00m
        };

        var policy6 = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerId = customer4.Id,
            PolicyNumber = "GLOBEX-POL-003",
            ExpirationDate = DateTime.UtcNow.Date.AddDays(5),
            PremiumAmount = 950.00m
        };

        // =========================
        // Add data
        // =========================

        db.Tenants.AddRange(
            tenant1,
            tenant2);

        db.Customers.AddRange(
            customer1,
            customer2,
            customer3,
            customer4);

        db.Policies.AddRange(
            policy1,
            policy2,
            policy3,
            policy4,
            policy5,
            policy6);

        await db.SaveChangesAsync();
    }
}