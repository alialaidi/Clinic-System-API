namespace ClinicApplication.Application.DTOs.Clients;

public class ClientMedicalRecordDTO
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime AppointmentDatetime { get; set; }
    public string? VisitDescription { get; set; }
    public string? Diagnosis { get; set; }
    public string? PatientInstructions { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
}
