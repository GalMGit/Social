using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Social.Identity.Infrastructure.Auth;

public sealed class PermissionPolicyProvider(
    IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    public override Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireClaim(
                PermissionClaim.Type, 
                policyName)
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}