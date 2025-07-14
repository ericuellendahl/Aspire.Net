using Microsoft.AspNetCore.Mvc;
using Aspire.Net.ApiService.Domain.Interfaces;
using Aspire.Net.ApiService.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Aspire.Net.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    /// Realiza login e retorna token JWT
    /// </summary>
    /// <param name="loginDto">Dados de login</param>
    /// <returns>Token JWT</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("Tentativa de login para usuário: {Username}", loginDto.Username);

            var token = await _authService.LoginAsync(loginDto);

            if (token == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            return Ok(new { token, message = "Login realizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    /// <param name="registerDto">Dados de registro</param>
    /// <returns>Confirmação de registro</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            _logger.LogInformation("Tentativa de registro para usuário: {Username}", registerDto.Username);

            var result = await _authService.RegisterAsync(registerDto);

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