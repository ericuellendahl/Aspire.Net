using Aspire.Net.ApiService.Domain.Entities;

namespace Aspire.Net.ApiService.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User> CreateAsync(User user, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken);
        Task<User?> FindUserByTokenAsync(string token, CancellationToken cancellationToken);
    }
}
