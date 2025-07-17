using System.Text.Json.Serialization;

namespace Aspire.Net.Web.DTOs.Auth
{
    public class AuthResponse
    {
        [JsonPropertyName("token")]
        public TokenResponse? Token { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class TokenResponse
    {
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }
    }
}
