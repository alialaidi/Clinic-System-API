using ClinicApplication.Application.DTOs.Appointments;
using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Application.Mappings.Appointments;

public static class AppointmentMappings
{
    public static Appointment ToEntity(this CreateAppointmentDTO createAppointmentDTO, int receptionistId)
    {
        ArgumentNullException.ThrowIfNull(createAppointmentDTO);

        return new Appointment
        {
            ClientId = createAppointmentDTO.ClientId,
            DoctorId = createAppointmentDTO.DoctorId,
            AppointmentDatetime = createAppointmentDTO.AppointmentDatetime,
            Status = 0,
            UpdatedByReceptionistId = receptionistId
        };
    }

    public static AppointmentDTO ToDTO(this Appointment appointment)
    {
        ArgumentNullException.ThrowIfNull(appointment);

        return new AppointmentDTO
        {
            Id = appointment.Id,
            ClientId = appointment.ClientId,
            ClientName = $"{appointment.Client.Person.FirstName} {appointment.Client.Person.LastName}".Trim(),
            ClientCode = appointment.Client.ClientCode,
            DoctorId = appointment.DoctorId,
            DoctorName = $"{appointment.Doctor.Person.FirstName} {appointment.Doctor.Person.LastName}".Trim(),
            AppointmentDatetime = appointment.AppointmentDatetime,
            Status = appointment.Status,
            PostponeReason = appointment.PostponeReason
        };
    }
}
