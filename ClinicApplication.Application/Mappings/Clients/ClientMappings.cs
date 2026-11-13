using ClinicApplication.Application.DTOs.Clients;
using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Application.Mappings.Clients;

public static class ClientMappings
{
    public static ClientDTO ToDTO(this Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        return new ClientDTO
        {
            Id = client.Id,
            ClientCode = client.ClientCode,
            FirstName = client.Person?.FirstName ?? string.Empty,
            LastName = client.Person?.LastName ?? string.Empty,
            DateOfBirth = client.Person?.DateOfBirth,
            Gender = client.Person?.Gender,
            Phone = client.Person?.Phone ?? string.Empty,
            Address = client.Person?.Address,
            Email = client.Person?.User?.Email
        };
    }
    public static ClientMedicalHistoryDTO ToMedicalHistoryDTO(this List<Appointment> appointments)
    {
        ArgumentNullException.ThrowIfNull(appointments);
        if (appointments.Count == 0)
            throw new ArgumentException("At least one appointment is required to create a medical history DTO.", nameof(appointments));

        var client = appointments[0].Client;
        return new ClientMedicalHistoryDTO
        {
            ClientCode = client.ClientCode,
            FirstName = client.Person.FirstName,
            LastName = client.Person.LastName,
            DateOfBirth = client.Person.DateOfBirth,
            Gender = client.Person.Gender,
            Visits = appointments.Select(appointment => new ClientVisitHistoryDTO
            {
                AppointmentId = appointment.Id,
                AppointmentDatetime = appointment.AppointmentDatetime,
                Status = appointment.Status,
                PostponeReason = appointment.PostponeReason,
                DoctorName = string.Join(" ", new[]
                {
                    appointment.Doctor.Person.FirstName,
                    appointment.Doctor.Person.LastName
                }.Where(name => !string.IsNullOrWhiteSpace(name))),
                MedicalRecord = appointment.MedicalRecord is null
                    ? null
                    : new MedicalRecordHistoryDTO
                    {
                        CreatedAt = appointment.MedicalRecord.CreatedAt,
                        VisitDescription = appointment.MedicalRecord.VisitDescription,
                        Diagnosis = appointment.MedicalRecord.Diagnosis,
                        Notes = appointment.MedicalRecord.Notes,
                        Prescriptions = appointment.MedicalRecord.Prescriptions
                            .Select(prescription => prescription.ToDTO())
                            .ToList()
                    }
            }).ToList()
        };
    }
    public static Person ToPerson(this CreateClientDTO createClientDTO)
    {
        ArgumentNullException.ThrowIfNull(createClientDTO);

        return new Person
        {
            FirstName = createClientDTO.FirstName,
            LastName = createClientDTO.LastName,
            DateOfBirth = createClientDTO.DateOfBirth,
            Gender = createClientDTO.Gender,
            Phone = createClientDTO.Phone,
            Address = createClientDTO.Address
        };
    }

    public static Client ToEntity(this CreateClientDTO createClientDTO, string clientCode, int receptionistId)
    {
        ArgumentNullException.ThrowIfNull(createClientDTO);

        return new Client
        {
            ClientCode = clientCode,
            RegisteredBy = receptionistId
        };
    }

    public static PrescriptionHistoryDTO ToDTO(this Prescription prescription)
    {
        ArgumentNullException.ThrowIfNull(prescription);

        return new PrescriptionHistoryDTO
        {
            CreatedAt = prescription.CreatedAt,
            Items = prescription.PrescriptionItems.Select(item => item.ToDTO()).ToList()
        };
    }

    public static PrescriptionItemHistoryDTO ToDTO(this PrescriptionItem prescriptionItem)
    {
        ArgumentNullException.ThrowIfNull(prescriptionItem);

        return new PrescriptionItemHistoryDTO
        {
            MedicationName = prescriptionItem.MedicationName,
            Dosage = prescriptionItem.Dosage,
            Frequency = prescriptionItem.Frequency,
            StartDate = prescriptionItem.StartDate,
            EndDate = prescriptionItem.EndDate,
            SpecialInstructions = prescriptionItem.SpecialInstructions
        };
    }
}


