namespace MultiTenantPolicyApi.Models;

public  class Policy
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public string PolicyNumber { get; set; } = string.Empty;

    public DateTime ExpirationDate { get; set; }

    public decimal PremiumAmount { get; set; }

    public Customer Customer { get; set; } = null!;
}