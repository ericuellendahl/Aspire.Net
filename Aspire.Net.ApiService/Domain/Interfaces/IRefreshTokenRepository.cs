using Aspire.Net.ApiService.Domain.Entities;

namespace Aspire.Net.ApiService.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<bool> DisableRefrshTokenByEmailAsync(string email);
        Task<bool> DisableRefrshTokenByTokenAsync(string token);
        Task<bool> InserRefreshTokenAsync(RefreshToken refreshToken, string email);
        Task<bool> IsRefreshTokenValidAsync(string token);
    }
}
