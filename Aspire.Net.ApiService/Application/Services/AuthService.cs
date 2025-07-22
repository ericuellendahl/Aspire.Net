using Aspire.Net.ApiService.Domain.DTOs;
using Aspire.Net.ApiService.Domain.Entities;
using Aspire.Net.ApiService.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aspire.Net.ApiService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email, cancellationToken);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Tentativa de login para usuário inexistente ou inativo: {Email}", loginDto.Email);
                return Result<LoginResponseDto>.Failure("Usuário não encontrado ou inativo");
            }

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Tentativa de login com senha incorreta para usuário: {Email}", loginDto.Email);
                return Result<LoginResponseDto>.Failure("Senha incorreta");
            }

            var token = GenerateJwtToken(user);
            var refreshTokenDto = GenerateRefrshToken();

            RefreshToken refreshToken = RefreshTokenMapper(refreshTokenDto);

            await _refreshTokenRepository.DisableRefrshTokenByEmailAsync(user.Email, cancellationToken);
            await _refreshTokenRepository.InserRefreshTokenAsync(refreshToken, user.Email, cancellationToken);

            _logger.LogInformation("Login bem-sucedido para usuário: {Email}", loginDto.Email);

            var response = new LoginResponseDto { AccessToken = token, RefreshToken = refreshTokenDto.Token };

            return Result<LoginResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o processo de login");
            throw;
        }
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar se usuário já existe
            if (await _userRepository.ExistsAsync(registerDto.Username, registerDto.Email, cancellationToken))
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
                Role = registerDto.Role ?? "User"
            };

            await _userRepository.CreateAsync(user, cancellationToken);

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

    public async Task<bool> ValidateUserAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        return user != null && user.IsActive && VerifyPassword(password, user.PasswordHash);
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return new() { Message = "Refresh token não fornecido", Success = false };


        var isValid = await _refreshTokenRepository.IsRefreshTokenValidAsync(refreshToken, cancellationToken);
        if (!isValid)
            return new() { Message = "Refresh token inválido", Success = false };

        var currentUser = await _userRepository.FindUserByTokenAsync(refreshToken, cancellationToken);
        if (currentUser is null)
            return new() { Message = "Not found refresh token", Success = false };

        var token = GenerateJwtToken(currentUser);
        var generationRefresToken = GenerateRefrshToken();

        var refreshTokenMapper = RefreshTokenMapper(generationRefresToken);

        await _refreshTokenRepository.DisableRefrshTokenByEmailAsync(currentUser.Email, cancellationToken);
        await _refreshTokenRepository.InserRefreshTokenAsync(refreshTokenMapper, currentUser.Email, cancellationToken);

        return new AuthResultDto { Token = token, RefreshToken = generationRefresToken.Token, Success = true, Message = "Sucesso" };

    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken)
        => await _refreshTokenRepository.DisableRefrshTokenByTokenAsync(refreshToken, cancellationToken);

    private string GenerateJwtToken(User user)
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
            new Claim(nameof(user.Id), user.Id.ToString()),
            new Claim(nameof(user.Username), user.Username),
            new Claim(nameof(user.Email), user.Email),
            new Claim(nameof(user.Role), user.Role),
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

    private static RefreshTokenDto GenerateRefrshToken()
    {
        return new RefreshTokenDto
        {
            Token = Guid.NewGuid().ToString(),
            Expiration = DateTime.UtcNow.AddMonths(1),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    private static RefreshToken RefreshTokenMapper(RefreshTokenDto refreshTokenDto)
    {
        return new RefreshToken
        {
            Token = refreshTokenDto.Token,
            Expiration = refreshTokenDto.Expiration,
            IsActive = refreshTokenDto.IsActive,
            CreatedAt = refreshTokenDto.CreatedAt
        };
    }
}