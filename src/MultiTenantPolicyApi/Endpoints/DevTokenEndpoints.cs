using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MultiTenantPolicyApi.Endpoints;

public static class DevTokenEndpoints
{
    // NOT for production — issues unsigned-trust dev tokens so the reviewer can test tenant isolation locally.
    public static void MapDevTokenEndpoints(this IEndpointRouteBuilder app, string signingKey)
    {
        app.MapGet("/dev/token", (Guid tenantId) =>
        {
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: new[] { new Claim("tenant_id", tenantId.ToString()) },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        })
        .WithSummary("DEV ONLY — mints a JWT for the given tenantId to simulate tenant context");
    }
}