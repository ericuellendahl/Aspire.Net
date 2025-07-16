using System.ComponentModel.DataAnnotations;

namespace Aspire.Net.ApiService.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(50)]
        public string Role { get; set; } = "User";

        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }
    }
}
