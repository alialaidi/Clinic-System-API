using ClinicApplication.Application.DTOs.Departments;
using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Application.Mappings.Departments;

public static class DepartmentMappings
{
    public static DepartmentDTO ToDTO(this Department department)
    {
        ArgumentNullException.ThrowIfNull(department);

        return new DepartmentDTO
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description
        };
    }

    public static Department ToEntity(this UpdateDepartmentDTO updateDepartmentDTO)
    {
        ArgumentNullException.ThrowIfNull(updateDepartmentDTO);

        return new Department
        {
            Name = updateDepartmentDTO.Name,
            Description = updateDepartmentDTO.Description
        };
    }

    public static Department ToEntity(this CreateDepartmentDTO createDepartmentDTO)
    {
        ArgumentNullException.ThrowIfNull(createDepartmentDTO);

        return new Department
        {
            Name = createDepartmentDTO.Name,
            Description = createDepartmentDTO.Description
        };
    }
}

