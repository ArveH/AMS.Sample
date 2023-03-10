using Microsoft.AspNetCore.Authorization;

namespace AMS.WebAPI.Authorization;

public class ReaderHandler : AuthorizationHandler<ReaderRequirement>
{
    private readonly ILogger<ReaderHandler> _logger;

    public ReaderHandler(ILogger<ReaderHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        ReaderRequirement requirement)
    {
        var u4Role = context.User.Claims.FirstOrDefault(c => c.Type == "u4_role")?.Value;
        if (string.IsNullOrWhiteSpace(u4Role))
        {
            _logger.LogError("The u4_role claim was not found, so authorization fails");
            return Task.CompletedTask;
        }

        _logger.LogInformation("User has the '{Role}' role", u4Role);
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}