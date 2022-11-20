using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AMS.WebApp.Controllers;

public class AccountController : Controller
{
    [HttpPost]
    //[ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync("oidc");
        HttpContext.SignOutAsync("Cookies");

        return RedirectToPage("/Index");
    }
}