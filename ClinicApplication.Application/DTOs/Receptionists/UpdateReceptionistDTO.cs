using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.Receptionists;

public class UpdateReceptionistDTO
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(1)]
    public string? Gender { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }
}
