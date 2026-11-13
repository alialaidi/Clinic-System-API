using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.MedicalRecords;

public class CreatePrescriptionDTO
{
    [Required]
    [MinLength(1)]
    public List<CreatePrescriptionItemDTO> Items { get; set; } = new();
}

public class CreatePrescriptionItemDTO
{
    [Required]
    [StringLength(100)]
    public string MedicationName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Frequency { get; set; } = string.Empty;

    [Required]
    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [StringLength(200)]
    public string? SpecialInstructions { get; set; }
}
