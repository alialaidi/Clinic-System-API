namespace ClinicApplication.Application.DTOs.Appointments;

public class AppointmentDTO
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientCode { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime AppointmentDatetime { get; set; }
    public byte Status { get; set; }
    public string? PostponeReason { get; set; }
}
