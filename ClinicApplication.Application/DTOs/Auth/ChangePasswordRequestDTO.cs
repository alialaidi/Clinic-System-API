using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.Auth;

public class ChangePasswordRequestDTO
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "The new password must contain at least 8 characters.")]
    public string NewPassword { get; set; } = string.Empty;
}
