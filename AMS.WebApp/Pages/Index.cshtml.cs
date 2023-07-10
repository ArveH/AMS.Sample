using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AMS.WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        if (User.Identity is { IsAuthenticated: false })
        {
            _logger.LogInformation("Logged out");
        }
    }
}