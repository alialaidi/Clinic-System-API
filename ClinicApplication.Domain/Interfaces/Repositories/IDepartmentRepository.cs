using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Domain.Interfaces.Repositories;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllDepartmentsAsync();
    Task<Department> CreateDepartmentAsync(Department department);
    Task<Department?> GetDepartmentForUpdateAsync(int departmentId);
    Task<bool> HasDoctorsAsync(int departmentId);
    void DeleteDepartment(Department department);
    Task<bool> DepartmentExistsAsync(int departmentId);
    Task<List<Doctor>> GetDoctorsForDepartmentAsync(int departmentId);
}



