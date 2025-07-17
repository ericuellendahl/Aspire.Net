namespace Aspire.Net.ApiService.Domain.DTOs
{
    public class RefreshTokenDto
    {
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
