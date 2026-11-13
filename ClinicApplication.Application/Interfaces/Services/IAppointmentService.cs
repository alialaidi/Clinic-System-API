using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Appointments;

namespace ClinicApplication.Application.Interfaces.Services;

public interface IAppointmentService
{
    Task<Result> CreateAppointmentAsync(CreateAppointmentDTO createAppointmentDTO, int userId);
    Task<List<AppointmentDTO>> GetAllAppointmentsAsync();
    Task<Result<AppointmentDTO>> GetAppointmentByIdAsync(int appointmentId, int userId, bool isReceptionist);
    Task<Result> PostponeAppointmentAsync(int appointmentId, PostponeAppointmentDTO request, int userId);
    Task<Result> CancelAppointmentAsync(int appointmentId, int userId);
    Task<Result> CompleteAppointmentAsync(int appointmentId, int userId);
}
