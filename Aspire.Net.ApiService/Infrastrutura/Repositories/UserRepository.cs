using Aspire.Net.ApiService.Domain.Entities;
using Aspire.Net.ApiService.Domain.Interfaces;
using Aspire.Net.ApiService.Infrastrutura.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Aspire.Net.ApiService.Infrastrutura.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Users
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por username: {Username}", username);
            return null;
        }
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Users
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por email: {Email}", email);
            return null;
        }
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário: {Username}", user.Username);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Users
                                 .AsNoTracking()
                                 .AnyAsync(u => u.Username == username || u.Email == email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar se usuário existe: {Username}, {Email}", username, email);
            return false;
        }
    }

    public async Task<User?> FindUserByTokenAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Users
                                 .Include(u => u.RefreshTokens)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == token), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por token: {Token}", token);
            return null;
        }
    }
}