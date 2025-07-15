using Aspire.Net.ApiService.Domain.DTOs;
using Aspire.Net.ApiService.Domain.Entities;
using Aspire.Net.ApiService.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Aspire.Net.ApiService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string?> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Tentativa de login para usuário inexistente ou inativo: {Username}", loginDto.Username);
                return null;
            }

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Tentativa de login com senha incorreta para usuário: {Username}", loginDto.Username);
                return null;
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation("Login bem-sucedido para usuário: {Username}", loginDto.Username);

            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o processo de login");
            return null;
        }
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Verificar se usuário já existe
            if (await _userRepository.ExistsAsync(registerDto.Username, registerDto.Email))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Usuário ou email já existem"
                };
            }

            // Criar novo usuário
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Role = "User"
            };

            await _userRepository.CreateAsync(user);

            _logger.LogInformation("Usuário registrado com sucesso: {Username}", registerDto.Username);

            return new AuthResultDto
            {
                Success = true,
                Message = "Usuário registrado com sucesso"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o registro do usuário");
            return new AuthResultDto
            {
                Success = false,
                Message = "Erro interno durante o registro"
            };
        }
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user != null && user.IsActive && VerifyPassword(password, user.PasswordHash);
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expireMinutes = int.Parse(jwtSettings["ExpireMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}