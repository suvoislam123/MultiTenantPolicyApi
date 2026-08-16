namespace MultiTenantPolicyApi.Models;

public sealed class Tenant
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Customer> Customers { get; set; } = [];
}