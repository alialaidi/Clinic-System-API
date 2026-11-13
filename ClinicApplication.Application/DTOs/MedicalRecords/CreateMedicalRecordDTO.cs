using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.MedicalRecords;

public class CreateMedicalRecordDTO
{
    [StringLength(1000)]
    public string? VisitDescription { get; set; }

    [StringLength(1000)]
    public string? Diagnosis { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(1000)]
    public string? PatientInstructions { get; set; }
}
