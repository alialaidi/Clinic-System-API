using ClinicApplication.Domain.Entities;
using ClinicApplication.Domain.Interfaces.Repositories;
using ClinicApplication.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicApplication.Infrastructure.Persistence.Repositories;

public class MedicalRecordRepository : IMedicalRecordRepository
{
    private readonly ClinicDbContext _context;

    public MedicalRecordRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public Task<MedicalRecord?> GetMedicalRecordForUpdateByDoctorAsync(int medicalRecordId, int doctorId)
        => _context.MedicalRecords
            .Include(medicalRecord => medicalRecord.Appointment)
            .FirstOrDefaultAsync(medicalRecord =>
                medicalRecord.Id == medicalRecordId &&
                medicalRecord.Appointment.DoctorId == doctorId);

    public async Task<Prescription> CreatePrescriptionAsync(Prescription prescription)
    {
        await _context.Prescriptions.AddAsync(prescription);
        return prescription;
    }
}
