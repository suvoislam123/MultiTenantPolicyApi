namespace MultiTenantPolicyApi.Auth;

public interface ITenantContext
{
    Guid TenantId { get; }
}