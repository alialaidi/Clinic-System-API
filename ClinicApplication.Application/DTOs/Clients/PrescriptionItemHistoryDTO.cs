namespace ClinicApplication.Application.DTOs.Clients;

public class PrescriptionItemHistoryDTO
{
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? SpecialInstructions { get; set; }
}
