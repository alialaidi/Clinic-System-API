namespace ClinicApplication.Application.DTOs.Clients;

public class CreateClientResponseDTO
{
    public string ClientCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TemporaryPassword { get; set; } = string.Empty;
}
