using Microsoft.AspNetCore.Authorization;

namespace AMS.WebAPI.Authorization;

public class ReaderHandler : AuthorizationHandler<ReaderRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, ReaderRequirement requirement)
    {
        if (context.User.Claims.Any() && !context.User.HasClaim(c => c.Type == "sub"))
        {
            // We assume this means the API is accessed with a ClientCredentials client,
            // so authorization succeeds
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var unit4Role = context.User.Claims.FirstOrDefault(c => c.Type == "unit4_role")?.Value;
        if (string.IsNullOrWhiteSpace(unit4Role))
        {
            // The unit4_role claim was not found, so authorization failed
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}