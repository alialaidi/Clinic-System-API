using ClinicApplication.Domain.Entities;
using ClinicApplication.Application.DTOs.Clients;

namespace ClinicApplication.Application.Mappings.Clients;

public static class ClientMedicalRecordMappings
{
    public static ClientMedicalRecordDTO ToClientDTO(this MedicalRecord medicalRecord)
    {
        ArgumentNullException.ThrowIfNull(medicalRecord);

        return new ClientMedicalRecordDTO
        {
            Id = medicalRecord.Id,
            AppointmentId = medicalRecord.AppointmentId,
            CreatedAt = medicalRecord.CreatedAt,
            AppointmentDatetime = medicalRecord.Appointment.AppointmentDatetime,
            VisitDescription = medicalRecord.VisitDescription,
            Diagnosis = medicalRecord.Diagnosis,
            PatientInstructions = medicalRecord.PatientInstructions,
            DoctorName = $"{medicalRecord.Appointment.Doctor.Person.FirstName} {medicalRecord.Appointment.Doctor.Person.LastName}".Trim(),
            DepartmentName = medicalRecord.Appointment.Doctor.Department.Name
        };
    }
}
