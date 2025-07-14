using Aspire.Net.ApiService.Domain.Entities;

namespace Aspire.Net.ApiService.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<bool> ExistsAsync(string username, string email);
    }
}
