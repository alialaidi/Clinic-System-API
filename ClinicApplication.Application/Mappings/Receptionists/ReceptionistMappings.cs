using ClinicApplication.Application.DTOs.Receptionists;
using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Application.Mappings.Receptionists;

public static class ReceptionistMappings
{
    public static Receptionist ToEntity(this CreateReceptionistDTO createReceptionistDTO, int adminId)
    {
        ArgumentNullException.ThrowIfNull(createReceptionistDTO);

        return new Receptionist
        {
            PersonId = createReceptionistDTO.PersonId,
            CreatedByAdminId = adminId
        };
    }

    public static ReceptionistDTO ToDTO(this Receptionist receptionist)
    {
        ArgumentNullException.ThrowIfNull(receptionist);

        return new ReceptionistDTO
        {
            Id = receptionist.Id,
            FirstName = receptionist.Person?.FirstName ?? string.Empty,
            LastName = receptionist.Person?.LastName ?? string.Empty,
            CreatedByAdminName = string.Join(" ",
                new[]
                {
                    receptionist.CreatedByAdmin?.Person?.FirstName,
                    receptionist.CreatedByAdmin?.Person?.LastName
                }.Where(name => !string.IsNullOrWhiteSpace(name)))
        };
    }
}
