namespace ClinicApplication.Application.DTOs.Clients;

public class PrescriptionHistoryDTO
{
    public DateTime CreatedAt { get; set; }
    public List<PrescriptionItemHistoryDTO> Items { get; set; } = new();
}
