using ClinicApplication.Application.DTOs.Doctors;
using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Application.Mappings.Doctors;

public static class DoctorMappings
{
    public static DoctorDTO ToDTO(this Doctor doctor)
    {
        ArgumentNullException.ThrowIfNull(doctor);

        return new DoctorDTO
        {
            Id = doctor.Id,
            FirstName = doctor.Person?.FirstName ?? string.Empty,
            LastName = doctor.Person?.LastName ?? string.Empty,

            CreatedByAdminName = string.Join(" ",
                new[]
                {
                doctor.CreatedByAdmin?.Person?.FirstName,
                doctor.CreatedByAdmin?.Person?.LastName
                }.Where(name => !string.IsNullOrWhiteSpace(name))),

            DepartmentName = doctor.Department?.Name ?? string.Empty,
            Specialization = doctor.Specialization
        };
    }


    public static Doctor ToEntity(this CreateDoctorDTO doctorCreateDto)
    {
        ArgumentNullException.ThrowIfNull(doctorCreateDto);
        return new Doctor
        {
            PersonId = doctorCreateDto.PersonId,
            DepartmentId = doctorCreateDto.DepartmentId,
            Specialization = doctorCreateDto.Specialization
        };
    }


    public static Doctor ToEntity(this UpdateDoctorDTO doctorUpdateDto)
    {
        ArgumentNullException.ThrowIfNull(doctorUpdateDto);
        return new Doctor
        {
            PersonId = doctorUpdateDto.PersonId,
            DepartmentId = doctorUpdateDto.DepartmentId,
            Specialization = doctorUpdateDto.Specialization
        };
    }



}
