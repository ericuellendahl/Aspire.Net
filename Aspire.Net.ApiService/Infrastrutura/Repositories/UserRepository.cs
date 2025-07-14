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

    public async Task<User?> GetByUsernameAsync(string username)
    {
        try
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por username: {Username}", username);
            return null;
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por email: {Email}", email);
            return null;
        }
    }

    public async Task<User> CreateAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário: {Username}", user.Username);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string username, string email)
    {
        try
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username || u.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar se usuário existe: {Username}, {Email}", username, email);
            return false;
        }
    }
}