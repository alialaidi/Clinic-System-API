using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.Appointments;

public class PostponeAppointmentDTO
{
    public DateTime AppointmentDatetime { get; set; }

    [StringLength(300, ErrorMessage = "PostponeReason cannot exceed 300 characters.")]
    public string? PostponeReason { get; set; }
}
