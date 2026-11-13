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
    Task<Appointment?> GetAppointmentForMedicalRecordAsync(int appointmentId, int doctorId);
    Task<bool> AppointmentExistsAsync(int appointmentId);
    Task<List<Payment>> GetAllPaymentsAsync();
    Task<List<Payment>> GetPaymentsByAppointmentIdAsync(int appointmentId);
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task<MedicalRecord> CreateMedicalRecordAsync(MedicalRecord medicalRecord);
    Task<bool> IsDoctorSlotAvailableAsync(int doctorId, DateTime appointmentDatetime);
    Task<bool> IsDoctorSlotAvailableAsync(int doctorId, DateTime appointmentDatetime, int exceptAppointmentId);
}
