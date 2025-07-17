using System.ComponentModel.DataAnnotations;

namespace Aspire.Net.ApiService.Domain.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "{0} é obrigatório")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} é obrigatório")]
        public string Password { get; set; } = string.Empty;
    }
}
