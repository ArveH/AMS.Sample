using AMS.Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Client.Controllers;

public class SecureController : Controller
{
    private readonly ILogger<SecureController> _logger;

    public SecureController(ILogger<SecureController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(new SecureViewModel() {UserName = "Arve"});
    }
}