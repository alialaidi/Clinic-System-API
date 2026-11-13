using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Departments;
using ClinicApplication.Application.Interfaces.Services;
using ClinicApplication.Application.Mappings.Departments;
using ClinicApplication.Domain.Interfaces.Repositories;

namespace ClinicApplication.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<DepartmentDTO>> GetAllDepartmentsAsync()
    {
        var departments = await _departmentRepository.GetAllDepartmentsAsync();
        return departments.Select(department => department.ToDTO()).ToList();
    }

    public async Task<Result> TransferDoctorsAsync(int sourceDepartmentId, int destinationDepartmentId)
    {
        var sourceDepartment = await _departmentRepository.GetDepartmentForUpdateAsync(sourceDepartmentId);
        if (sourceDepartment is null)
            return Result.NotFound("The source department was not found");

        if (!await _departmentRepository.DepartmentExistsAsync(destinationDepartmentId))
            return Result.NotFound("The destination department was not found");

        if (sourceDepartmentId == destinationDepartmentId)
            return Result.BadRequest("The source and destination departments must be different");

        var doctors = await _departmentRepository.GetDoctorsForDepartmentAsync(sourceDepartmentId);
        foreach (var doctor in doctors)
            doctor.DepartmentId = destinationDepartmentId;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
    public async Task<Result> DeleteDepartmentAsync(int departmentId)
    {
        var department = await _departmentRepository.GetDepartmentForUpdateAsync(departmentId);
        if (department is null)
            return Result.NotFound("The department was not found");

        if (await _departmentRepository.HasDoctorsAsync(departmentId))
            return Result.Conflict("Doctors must be transferred to another department before this department can be deleted");

        _departmentRepository.DeleteDepartment(department);
        await _unitOfWork.CommitAsync();
        return Result.Success();
    }

    public async Task<Result> UpdateDepartmentAsync(int departmentId, UpdateDepartmentDTO updateDepartmentDTO)
    {
        var department = await _departmentRepository.GetDepartmentForUpdateAsync(departmentId);
        if (department is null)
            return Result.NotFound("The department was not found");

        department.Name = updateDepartmentDTO.Name;
        department.Description = updateDepartmentDTO.Description;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }

    public async Task<Result> CreateDepartmentAsync(CreateDepartmentDTO createDepartmentDTO)
    {
        var department = createDepartmentDTO.ToEntity();
        await _departmentRepository.CreateDepartmentAsync(department);
        await _unitOfWork.CommitAsync();
        return Result.Success(department.Id);
    }
}



