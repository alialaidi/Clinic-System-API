using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.MedicalRecords;
using ClinicApplication.Application.Interfaces.Services;
using ClinicApplication.Application.Mappings.MedicalRecords;
using ClinicApplication.Domain.Interfaces.Repositories;

namespace ClinicApplication.Application.Services;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MedicalRecordService(
        IMedicalRecordRepository medicalRecordRepository,
        IDoctorRepository doctorRepository,
        IUnitOfWork unitOfWork)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> UpdateMedicalRecordAsync(
        int medicalRecordId,
        UpdateMedicalRecordDTO updateMedicalRecordDTO,
        int userId)
    {
        var doctorId = await _doctorRepository.GetDoctorIdByUserIdAsync(userId);
        if (doctorId is null)
            return Result.NotFound("The doctor was not found");

        var medicalRecord = await _medicalRecordRepository.GetMedicalRecordForUpdateByDoctorAsync(
            medicalRecordId,
            doctorId.Value);
        if (medicalRecord is null)
            return Result.NotFound("The medical record was not found");

        medicalRecord.VisitDescription = updateMedicalRecordDTO.VisitDescription;
        medicalRecord.Diagnosis = updateMedicalRecordDTO.Diagnosis;
        medicalRecord.Notes = updateMedicalRecordDTO.Notes;
        medicalRecord.PatientInstructions = updateMedicalRecordDTO.PatientInstructions;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }

    public async Task<Result> CreatePrescriptionAsync(
        int medicalRecordId,
        CreatePrescriptionDTO createPrescriptionDTO,
        int userId)
    {
        var doctorId = await _doctorRepository.GetDoctorIdByUserIdAsync(userId);
        if (doctorId is null)
            return Result.NotFound("The doctor was not found");

        var medicalRecord = await _medicalRecordRepository.GetMedicalRecordForUpdateByDoctorAsync(
            medicalRecordId,
            doctorId.Value);
        if (medicalRecord is null)
            return Result.NotFound("The medical record was not found");

        var prescription = createPrescriptionDTO.ToEntity(medicalRecord.Id);
        await _medicalRecordRepository.CreatePrescriptionAsync(prescription);
        await _unitOfWork.CommitAsync();

        return Result.Success(prescription.Id);
    }
}
