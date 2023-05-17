using Common.Identity.Models;
using Common.Identity.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Common.Identity.Policies.Handlers;

public sealed class ScopePolicyHandler : AuthorizationHandler<ScopePolicyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ScopePolicyRequirement requirement)
    {
        // Check claim
        var claim = context.User.FindFirst(claim => claim.Type == CustomClaims.Scope.Type && claim.Value == CustomClaims.Scope.Value);
        if (claim is not null)
        {
            context.Succeed(requirement);
        }
        
        // Check other claims if needed
        return Task.CompletedTask;
    }
}