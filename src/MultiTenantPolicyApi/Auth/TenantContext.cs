namespace MultiTenantPolicyApi.Auth;

public class TenantContext : ITenantContext
{
    public Guid TenantId { get; }

    public TenantContext(IHttpContextAccessor accessor)
    {
        var claim = accessor.HttpContext?.User.FindFirst("tenant_id")
            ?? throw new UnauthorizedAccessException("Missing tenant_id claim.");

        TenantId = Guid.Parse(claim.Value);
    }
}