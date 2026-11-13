using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.MedicalRecords;

namespace ClinicApplication.Application.Interfaces.Services;

public interface IMedicalRecordService
{
    Task<Result> UpdateMedicalRecordAsync(int medicalRecordId, UpdateMedicalRecordDTO updateMedicalRecordDTO, int userId);
    Task<Result> CreatePrescriptionAsync(int medicalRecordId, CreatePrescriptionDTO createPrescriptionDTO, int userId);
}
