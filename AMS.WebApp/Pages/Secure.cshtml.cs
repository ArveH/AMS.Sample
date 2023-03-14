using AMS.WebApp.Data;
using AMS.WebApp.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AMS.WebApp.Pages;

public class SecureModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SecureModel> _logger;

    public SecureModel(
        IHttpClientFactory httpClientFactory,
        ILogger<SecureModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public List<WeatherForecast>? Forecast { get; set; } = new();
    public string? ErrorMsg { get; set; }

    public void OnGet()
    {
        _logger.LogInformation("Logged in");
    }

    public async Task<IActionResult> OnPostGetData()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        ArgumentException.ThrowIfNullOrEmpty(accessToken);
        _logger.LogInformation("Access token: " + accessToken);

        accessToken = await UpgradeAccessTokenAsync(accessToken);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            ErrorMsg = "Upgrading access token failed";
            return StatusCode(500, ErrorMsg);
        }

        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        _logger.LogInformation("Getting weather forecast");
        var response = await httpClient.GetAsync(WebAppConstants.ApiUrl + "/WeatherForecast");
        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "Can't get WeatherForecast";
            }
            ErrorMsg = msg;
            _logger.LogInformation("Getting weather forecast failed: " + msg);
            return StatusCode(500, msg);
        }

        _logger.LogInformation("Getting weather forecast succeeded");
        var jsonTxt = await response.Content.ReadAsStringAsync();
        Forecast = JsonSerializer.Deserialize<List<WeatherForecast>>(
            jsonTxt, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        _logger.LogInformation("Weather forecast deserialized");

        return Page();
    }

    private async Task<string?> UpgradeAccessTokenAsync(string accessToken)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        _logger.LogInformation("Upgrading access token...");
        var response = await httpClient.GetAsync(
            WebAppConstants.AmsBaseUrl + 
            "/am-token-upgrade/human-involved-flow/" + WebAppConstants.AmsSourceSystem);
        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "Can't get upgraded token";
            }
            _logger.LogInformation("Upgrading access token failed: " + msg);
            return null;
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        return tokenResponse?.AccessToken;
    }

    public IActionResult OnGetLogout()
    {
        _logger.LogInformation("Logging out...");
        return SignOut("Cookies", "oidc");
    }
}