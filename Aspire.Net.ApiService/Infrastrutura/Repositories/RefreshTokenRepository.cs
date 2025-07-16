using Aspire.Net.ApiService.Domain.Entities;
using Aspire.Net.ApiService.Domain.Interfaces;
using Aspire.Net.ApiService.Infrastrutura.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Aspire.Net.ApiService.Infrastrutura.Repositories
{
    public class RefreshTokenRepository(ApplicationDbContext context, ILogger<UserRepository> logger) : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<UserRepository> _logger = logger;


        public async Task<bool> InserRefreshTokenAsync(RefreshToken refreshToken, string email, CancellationToken cancellationToken)
        {
            try
            {
                refreshToken.Email = email;

                await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir Refresh Token: {Token}", refreshToken.Token);
                throw;
            }
        }

        public async Task<bool> DisableRefrshTokenByEmailAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(e => e.Email == email);
                if (refreshToken == null) return false;

                refreshToken.IsActive = false;

                _context.RefreshTokens.Update(refreshToken);
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar E-mail: {Email}", email);
                throw;
            }
        }

        public async Task<bool> DisableRefrshTokenByTokenAsync(string token, CancellationToken cancellationToken)
        {
            try
            {
                var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(e => e.Token == token);
                if (refreshToken == null) return false;

                refreshToken.IsActive = false;

                _context.RefreshTokens.Update(refreshToken);
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar RefreshToken: {Token}", token);
                throw;
            }
        }

        public async Task<bool> IsRefreshTokenValidAsync(string token, CancellationToken cancellationToken)
        {
            try
            {
                var refreshToken = await _context.RefreshTokens
                                                 .AsNoTracking()
                                                 .FirstOrDefaultAsync(e => e.Token == token && e.IsActive && e.Expiration >= DateTime.UtcNow, cancellationToken);
                return refreshToken != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar Refresh Token: {Token}", token);
                throw;
            }
        }

    }
}
