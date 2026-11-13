namespace ClinicApplication.Application.DTOs.Clients;

public class ClientVisitHistoryDTO
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentDatetime { get; set; }
    public byte Status { get; set; }
    public string? PostponeReason { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public MedicalRecordHistoryDTO? MedicalRecord { get; set; }
}
