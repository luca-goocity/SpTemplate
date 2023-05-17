using Common.Identity.Models;
using Common.Identity.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Common.Identity.Policies.Handlers;

public sealed class CanEditPolicyHandler : AuthorizationHandler<CanEditPolicyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        CanEditPolicyRequirement requirement)
    {
        // Check claim
        var claim = context.User.FindFirst(claim => claim.Type == CustomClaims.CanDelete.Type);
        if (claim is not null && bool.TryParse(claim.Value, out var canView) && canView)
        {
            context.Succeed(requirement);
        }
        
        // Check other claims if needed
        return Task.CompletedTask;
    }
}