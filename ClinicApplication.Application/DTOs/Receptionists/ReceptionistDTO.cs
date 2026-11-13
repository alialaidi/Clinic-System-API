namespace ClinicApplication.Application.DTOs.Receptionists;

public class ReceptionistDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string CreatedByAdminName { get; set; } = string.Empty;
}
