using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Domain.Interfaces.Repositories;

public interface IAppointmentRepository
{
    Task<bool> ClientExistsAsync(int clientId);
    Task<bool> DoctorExistsAsync(int doctorId);
    Task<int?> GetReceptionistIdByUserIdAsync(int userId);
    Task<Appointment> CreateAppointmentAsync(Appointment appointment);
    Task<List<Appointment>> GetAllAppointmentsAsync();
    Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);
    Task<Appointment?> GetAppointmentByIdForDoctorAsync(int appointmentId, int doctorId);
    Task<Appointment?> GetAppointmentForUpdateAsync(int appointmentId);
    Task<Appointment?> GetAppointmentForUpdateByDoctorAsync(int appointmentId, int doctorId);
    Task<bool> IsDoctorSlotAvailableAsync(int doctorId, DateTime appointmentDatetime);
    Task<bool> IsDoctorSlotAvailableAsync(int doctorId, DateTime appointmentDatetime, int exceptAppointmentId);
}
