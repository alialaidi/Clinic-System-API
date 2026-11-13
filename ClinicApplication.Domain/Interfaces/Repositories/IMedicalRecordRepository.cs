using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Domain.Interfaces.Repositories;

public interface IMedicalRecordRepository
{
    Task<MedicalRecord?> GetMedicalRecordForUpdateByDoctorAsync(int medicalRecordId, int doctorId);
    Task<Prescription> CreatePrescriptionAsync(Prescription prescription);
}
