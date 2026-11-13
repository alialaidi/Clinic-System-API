namespace ClinicApplication.Application.DTOs.Clients;

public class ClientDTO
{
    public int Id { get; set; }
    public string ClientCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Email { get; set; }
}
