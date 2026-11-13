using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Departments;

namespace ClinicApplication.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<List<DepartmentDTO>> GetAllDepartmentsAsync();
    Task<Result> CreateDepartmentAsync(CreateDepartmentDTO createDepartmentDTO);
    Task<Result> UpdateDepartmentAsync(int departmentId, UpdateDepartmentDTO updateDepartmentDTO);
    Task<Result> DeleteDepartmentAsync(int departmentId);
    Task<Result> TransferDoctorsAsync(int sourceDepartmentId, int destinationDepartmentId);
}



