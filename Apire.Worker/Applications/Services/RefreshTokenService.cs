using Apire.Worker.Domain.Interfaces;
using Apire.Worker.Infraestrutura.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Apire.Worker.Applications.Services
{
    public class RefreshTokenService(ApplicationDbContext dbContext) : IRefreshTokenService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task DeleteAllrefreshToken(CancellationToken cancellationToken)
        {
            try
            {
                var refreshTokens = await _dbContext.RefreshTokens.Where(s => s.CreatedAt < DateTime.UtcNow.Date).ToListAsync(cancellationToken);
                if (refreshTokens.Count != 0)
                {
                    _dbContext.RefreshTokens.RemoveRange(refreshTokens);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar os tokens de atualização", ex);
            }
        }
    }
}
