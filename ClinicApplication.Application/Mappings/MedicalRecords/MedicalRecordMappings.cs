using ClinicApplication.Application.DTOs.MedicalRecords;
using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Application.Mappings.MedicalRecords;

public static class MedicalRecordMappings
{
    public static MedicalRecord ToEntity(this CreateMedicalRecordDTO createMedicalRecordDTO, int appointmentId)
    {
        ArgumentNullException.ThrowIfNull(createMedicalRecordDTO);

        return new MedicalRecord
        {
            AppointmentId = appointmentId,
            VisitDescription = createMedicalRecordDTO.VisitDescription,
            Diagnosis = createMedicalRecordDTO.Diagnosis,
            Notes = createMedicalRecordDTO.Notes,
            PatientInstructions = createMedicalRecordDTO.PatientInstructions
        };
    }
}
