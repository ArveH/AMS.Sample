using Microsoft.AspNetCore.Authorization;

namespace AMS.WebAPI.Authorization;

public class ReaderHandler : AuthorizationHandler<ReaderRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, ReaderRequirement requirement)
    {
        var unit4Role = context.User.Claims.FirstOrDefault(c => c.Type == "u4_role")?.Value;
        if (string.IsNullOrWhiteSpace(unit4Role))
        {
            // The unit4_role claim was not found, so authorization failed
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}