
namespace Apire.Worker.Domain.Interfaces
{
    public interface IRefreshTokenService
    {
        Task DeleteAllrefreshToken(CancellationToken cancellationToken);
    }
}
