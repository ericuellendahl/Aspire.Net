using System.ComponentModel.DataAnnotations;

namespace Aspire.Net.ApiService.Domain.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username é obrigatório")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password é obrigatório")]
        public string Password { get; set; } = string.Empty;
    }
}
