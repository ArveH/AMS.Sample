using System.Text.Json.Serialization;

namespace AMS.WebApp.Dtos;

public class TokenResponseDto
{
    public TokenResponseDto() { }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}