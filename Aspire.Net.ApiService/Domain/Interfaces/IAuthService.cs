using Aspire.Net.ApiService.Domain.DTOs;
using Aspire.Net.ApiService.Domain.Entities;

namespace Aspire.Net.ApiService.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> ValidateUserAsync(string username, string password);
        string GenerateJwtToken(User user);
    }
}
