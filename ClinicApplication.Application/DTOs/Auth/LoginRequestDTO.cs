using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.Auth;

public class LoginRequestDTO
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
