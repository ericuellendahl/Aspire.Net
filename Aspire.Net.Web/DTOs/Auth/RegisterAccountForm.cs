using System.ComponentModel.DataAnnotations;

namespace Aspire.Net.Web.DTOs.Auth
{
    public class RegisterAccountForm
    {
        [Required]
        [StringLength(6, ErrorMessage = "Name length can't be more than 6.")]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(8, ErrorMessage = "Password must be at least 6 characters long.", MinimumLength = 6)]
        public string? Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

        [Required]
        [StringLength(8, ErrorMessage = "{0} must be at least 8 characters long.", MinimumLength = 4)]
        public string? Role { get; set; }
    }
}
