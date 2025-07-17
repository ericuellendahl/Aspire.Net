namespace Aspire.Net.ApiService.Domain.DTOs
{
    public class LoginResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
