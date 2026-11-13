using ClinicApplication.Domain.Entities;
using ClinicApplication.Domain.Interfaces.Repositories;
using ClinicApplication.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicApplication.Infrastructure.Persistence.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ClinicDbContext _context;

    public DepartmentRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public Task<List<Department>> GetAllDepartmentsAsync()
    {
        return _context.Departments
            .AsNoTracking()
            .Select(department => new Department
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            })
            .ToListAsync();
    }

    public Task<Department?> GetDepartmentForUpdateAsync(int departmentId)
        => _context.Departments
            .FirstOrDefaultAsync(department => department.Id == departmentId);

    public Task<bool> DepartmentExistsAsync(int departmentId)
        => _context.Departments.AnyAsync(department => department.Id == departmentId);

    public Task<List<Doctor>> GetDoctorsForDepartmentAsync(int departmentId)
        => _context.Doctors
            .Where(doctor => doctor.DepartmentId == departmentId)
            .ToListAsync();

    public Task<bool> HasDoctorsAsync(int departmentId)
        => _context.Doctors.AnyAsync(doctor => doctor.DepartmentId == departmentId);

    public void DeleteDepartment(Department department)
        => _context.Departments.Remove(department);

    public async Task<Department> CreateDepartmentAsync(Department department)
    {
        await _context.Departments.AddAsync(department);
        return department;
    }
}



