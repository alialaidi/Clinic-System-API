namespace ClinicApplication.Application.DTOs.Clients;

public class MedicalRecordHistoryDTO
{
    public DateTime CreatedAt { get; set; }
    public string? VisitDescription { get; set; }
    public string? Diagnosis { get; set; }
    public string? Notes { get; set; }
    public List<PrescriptionHistoryDTO> Prescriptions { get; set; } = new();
}
