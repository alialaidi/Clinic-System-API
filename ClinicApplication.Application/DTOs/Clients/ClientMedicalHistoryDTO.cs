namespace ClinicApplication.Application.DTOs.Clients;

public class ClientMedicalHistoryDTO
{
    public string ClientCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public List<ClientVisitHistoryDTO> Visits { get; set; } = new();
}
