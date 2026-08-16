namespace MultiTenantPolicyApi.Models;

public  class Customer
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public Tenant Tenant { get; set; } = null!;

    public ICollection<Policy> Policies { get; set; } = [];
}