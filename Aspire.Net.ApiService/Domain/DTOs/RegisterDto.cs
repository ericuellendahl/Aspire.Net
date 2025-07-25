﻿using System.ComponentModel.DataAnnotations;

namespace Aspire.Net.ApiService.Domain.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Username é obrigatório")]
    [MinLength(3, ErrorMessage = "Username deve ter pelo menos 3 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password é obrigatório")]
    [MinLength(6, ErrorMessage = "Password deve ter pelo menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirmação de password é obrigatória")]
    [Compare("Password", ErrorMessage = "Passwords não coincidem")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MaxLength(5, ErrorMessage = "{0} só permite até 5 carateres.")]
    public string? Role { get; set; }
}
