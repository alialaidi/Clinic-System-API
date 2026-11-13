using System.ComponentModel.DataAnnotations;

namespace ClinicApplication.Application.DTOs.Appointments
{
    public class CreateAppointmentDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "ClientId must be greater than zero.")]
        public int ClientId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "DoctorId must be greater than zero.")]
        public int DoctorId { get; set; }

        public DateTime AppointmentDatetime { get; set; }
    }
}
