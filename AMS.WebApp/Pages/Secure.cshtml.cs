using System.Net.Http.Headers;
using System.Text.Json;
using AMS.WebApp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;

namespace AMS.WebApp.Pages;

public class SecureModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SecureModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<WeatherForecast>? Forecast { get; set; } = new();
    public string? ErrorMsg { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostGetData()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://localhost:7053");
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        ArgumentException.ThrowIfNullOrEmpty(accessToken);

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.GetAsync("/WeatherForecast");
        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "Can't get WeatherForecast";
            }
            ErrorMsg = msg;
            return StatusCode(500, msg);
        }

        var jsonTxt = await response.Content.ReadAsStringAsync();
        Forecast = JsonSerializer.Deserialize<List<WeatherForecast>>(
            jsonTxt, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return Page();
    }

    public IActionResult OnGetLogout()
    {
        return SignOut("Cookies", "oidc");
    }
}