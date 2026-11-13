using ClinicApplication.Domain.Entities;
using ClinicApplication.Domain.Enums;
using ClinicApplication.Domain.Interfaces.Repositories;
using ClinicApplication.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicApplication.Infrastructure.Persistence.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicDbContext _context;

    public AppointmentRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public Task<bool> ClientExistsAsync(int clientId)
        => _context.Clients.AnyAsync(c => c.Id == clientId);

    public Task<bool> DoctorExistsAsync(int doctorId)
        => _context.Doctors.AnyAsync(d => d.Id == doctorId);

    public Task<int?> GetReceptionistIdByUserIdAsync(int userId)
        => _context.Users
            .Where(u => u.Id == userId)
            .Join(_context.Receptionists,
                u => u.PersonId,
                r => r.PersonId,
                (u, r) => (int?)r.Id)
            .FirstOrDefaultAsync();

    public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
        return appointment;
    }

    public async Task<List<Appointment>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
                .ThenInclude(c => c.Person)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Person)
            .OrderBy(a => a.AppointmentDatetime)
            .ToListAsync();
    }

    public Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        => _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
                .ThenInclude(c => c.Person)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Person)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

    public Task<Appointment?> GetAppointmentByIdForDoctorAsync(int appointmentId, int doctorId)
        => _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
                .ThenInclude(c => c.Person)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Person)
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == doctorId);

    public Task<Appointment?> GetAppointmentForUpdateAsync(int appointmentId)
        => _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);

    public Task<Appointment?> GetAppointmentForUpdateByDoctorAsync(int appointmentId, int doctorId)
        => _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == doctorId);

    public Task<Appointment?> GetAppointmentForMedicalRecordAsync(int appointmentId, int doctorId)
        => _context.Appointments
            .Include(a => a.MedicalRecord)
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == doctorId);

    public Task<bool> AppointmentExistsAsync(int appointmentId)
        => _context.Appointments.AnyAsync(a => a.Id == appointmentId);

    public Task<List<Payment>> GetAllPaymentsAsync()
        => _context.Payments
            .AsNoTracking()
            .ToListAsync();

    public Task<List<Payment>> GetPaymentsByAppointmentIdAsync(int appointmentId)
        => _context.Payments
            .AsNoTracking()
            .Where(p => p.AppointmentId == appointmentId)
            .ToListAsync();

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        return payment;
    }

    public async Task<MedicalRecord> CreateMedicalRecordAsync(MedicalRecord medicalRecord)
    {
        await _context.MedicalRecords.AddAsync(medicalRecord);
        return medicalRecord;
    }

    public async Task<bool> IsDoctorSlotAvailableAsync(int doctorId, DateTime appointmentDatetime)
        => !await _context.Appointments.AnyAsync(a =>
            a.DoctorId == doctorId &&
            a.AppointmentDatetime == appointmentDatetime &&
            a.Status != (byte)AppointmentStatus.Cancelled);

    public async Task<bool> IsDoctorSlotAvailableAsync(int doctorId, DateTime appointmentDatetime, int exceptAppointmentId)
        => !await _context.Appointments.AnyAsync(a =>
            a.DoctorId == doctorId &&
            a.AppointmentDatetime == appointmentDatetime &&
            a.Id != exceptAppointmentId &&
            a.Status != (byte)AppointmentStatus.Cancelled);
}
