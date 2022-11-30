using Microsoft.AspNetCore.Authorization;

namespace AMS.WebAPI.Authorization;

public class ReaderRequirement : IAuthorizationRequirement
{
    public string Role => "Reader";
}