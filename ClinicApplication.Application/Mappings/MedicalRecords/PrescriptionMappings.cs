using ClinicApplication.Application.DTOs.MedicalRecords;
using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Application.Mappings.MedicalRecords;

public static class PrescriptionMappings
{
    public static Prescription ToEntity(this CreatePrescriptionDTO createPrescriptionDTO, int medicalRecordId)
    {
        ArgumentNullException.ThrowIfNull(createPrescriptionDTO);

        return new Prescription
        {
            MedicalRecordId = medicalRecordId,
            PrescriptionItems = createPrescriptionDTO.Items.Select(item => new PrescriptionItem
            {
                MedicationName = item.MedicationName,
                Dosage = item.Dosage,
                Frequency = item.Frequency,
                StartDate = item.StartDate!.Value,
                EndDate = item.EndDate,
                SpecialInstructions = item.SpecialInstructions
            }).ToList()
        };
    }
}
