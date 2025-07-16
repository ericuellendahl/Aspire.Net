using Aspire.Net.ApiService.Domain.Entities;

namespace Aspire.Net.ApiService.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<bool> DisableRefrshTokenByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> DisableRefrshTokenByTokenAsync(string token, CancellationToken cancellationToken);
        Task<bool> InserRefreshTokenAsync(RefreshToken refreshToken, string email, CancellationToken cancellationToken);
        Task<bool> IsRefreshTokenValidAsync(string token, CancellationToken cancellationToken);
    }
}
