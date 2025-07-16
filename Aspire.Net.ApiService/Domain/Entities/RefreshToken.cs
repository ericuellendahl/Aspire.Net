using System.ComponentModel.DataAnnotations;

namespace Aspire.Net.ApiService.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? Email { get; set; }
        public User? User { get; set; }
    }
}
