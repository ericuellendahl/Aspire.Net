using Aspire.Net.ApiService.Domain.DTOs;
using Aspire.Net.ApiService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aspire.Net.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tentativa de login para usuário: {Email}", loginDto.Email);

        var result = await _authService.LoginAsync(loginDto, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("Falha no login para usuário {Email}: {Erro}", loginDto.Email, result.Error);
            return Unauthorized(new { message = result.Error });
        }

        return Ok(new { token = result.Value, message = "Login realizado com sucesso" });
    }


    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    /// <param name="registerDto">Dados de registro</param>
    /// <returns>Confirmação de registro</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Tentativa de registro para usuário: {Username}", registerDto.Username);

            var result = await _authService.RegisterAsync(registerDto, cancellationToken);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = "Usuário registrado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o registro");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshtoken"] ?? string.Empty;

        var result = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshtoken"] ?? string.Empty;

        if (!string.IsNullOrEmpty(refreshToken))
            await _authService.LogoutAsync(refreshToken, cancellationToken);

        return Ok(new { message = "Logout realizado com sucesso" });
    }


    /// <summary>
    /// Endpoint protegido para teste
    /// </summary>
    /// <returns>Informações do usuário logado</returns>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var username = User.Identity?.Name;
        var claims = User.Claims.Select(c => new { c.Type, c.Value });

        return Ok(new { username, claims });
    }
}