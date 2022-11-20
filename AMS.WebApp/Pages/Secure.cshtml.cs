using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AMS.WebApp.Pages;

public class SecureModel : PageModel
{

    public SecureModel()
    {
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostGetData()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        return Page();
    }

    public IActionResult OnGetLogout()
    {
        return SignOut("Cookies", "oidc");
    }
}