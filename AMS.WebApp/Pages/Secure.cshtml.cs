using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AMS.WebApp.Pages;

public class SecureModel : PageModel
{
    public string? UserName { get; set; }

    public void OnGet()
    {
        UserName = User.Claims.FirstOrDefault(c => c.Type == "unit4_id")?.Value ?? "<null> (can't find unit4_id claim)";
    }

    public IActionResult OnGetLogout()
    {
        return SignOut("Cookies", "oidc");
    }
}