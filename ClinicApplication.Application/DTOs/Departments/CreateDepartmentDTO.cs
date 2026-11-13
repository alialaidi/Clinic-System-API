using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.Departments;

public class CreateDepartmentDTO
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }
}
