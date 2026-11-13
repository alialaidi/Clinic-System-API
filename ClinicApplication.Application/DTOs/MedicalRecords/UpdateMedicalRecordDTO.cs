using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.MedicalRecords;

public class UpdateMedicalRecordDTO
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
