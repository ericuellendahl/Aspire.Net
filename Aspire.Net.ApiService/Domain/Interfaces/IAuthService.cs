using Aspire.Net.ApiService.Domain.DTOs;

namespace Aspire.Net.ApiService.Domain.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken);
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<AuthResultDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);
    Task<bool> ValidateUserAsync(string username, string password, CancellationToken cancellationToken);
}
