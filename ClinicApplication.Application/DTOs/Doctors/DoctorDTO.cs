namespace ClinicApplication.Application.DTOs.Doctors;

public class DoctorDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string CreatedByAdminName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
}


