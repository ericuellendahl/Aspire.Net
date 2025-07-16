using Aspire.Net.ApiService.Domain.DTOs;

namespace Aspire.Net.ApiService.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task LogoutAsync(string refreshToken);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> ValidateUserAsync(string username, string password);
    }
}
