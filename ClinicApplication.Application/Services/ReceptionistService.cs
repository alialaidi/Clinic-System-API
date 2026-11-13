using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Receptionists;
using ClinicApplication.Application.Interfaces.Services;
using ClinicApplication.Application.Mappings.Receptionists;
using ClinicApplication.Domain.Interfaces.Repositories;

namespace ClinicApplication.Application.Services;

public class ReceptionistService : IReceptionistService
{
    private readonly IReceptionistRepository _receptionistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReceptionistService(IReceptionistRepository receptionistRepository, IUnitOfWork unitOfWork)
    {
        _receptionistRepository = receptionistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ReceptionistDTO>> GetAllReceptionistsAsync()
    {
        var receptionists = await _receptionistRepository.GetAllReceptionistsAsync();
        return receptionists.Select(receptionist => receptionist.ToDTO()).ToList();
    }

    public async Task<Result> UpdateReceptionistAsync(int receptionistId, UpdateReceptionistDTO updateReceptionistDTO)
    {
        var receptionist = await _receptionistRepository.GetReceptionistForUpdateAsync(receptionistId);
        if (receptionist is null)
            return Result.NotFound("The receptionist was not found");

        receptionist.Person.FirstName = updateReceptionistDTO.FirstName;
        receptionist.Person.LastName = updateReceptionistDTO.LastName;
        receptionist.Person.Phone = updateReceptionistDTO.Phone;
        receptionist.Person.DateOfBirth = updateReceptionistDTO.DateOfBirth;
        receptionist.Person.Gender = updateReceptionistDTO.Gender;
        receptionist.Person.Address = updateReceptionistDTO.Address;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }

    public async Task<Result> UpdateStatusAsync(int receptionistId, bool isActive)
    {
        var user = await _receptionistRepository.GetUserByReceptionistIdAsync(receptionistId);
        if (user is null)
            return Result.NotFound("The receptionist was not found");

        if (user.IsActive == isActive)
            return Result.BadRequest(isActive ? "The receptionist is already active" : "The receptionist is already inactive");

        user.IsActive = isActive;
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }

    public async Task<Result> CreateReceptionistAsync(CreateReceptionistDTO createReceptionistDTO, int userId)
    {
        if (!await _receptionistRepository.PersonExistsAsync(createReceptionistDTO.PersonId))
            return Result.NotFound("The specified person was not found");

        if (await _receptionistRepository.ReceptionistExistsForPersonAsync(createReceptionistDTO.PersonId))
            return Result.Conflict("The person is already registered as a receptionist");

        var adminId = await _receptionistRepository.GetAdminIdByUserIdAsync(userId);
        if (adminId is null)
            return Result.NotFound("The authenticated admin was not found");

        var receptionist = createReceptionistDTO.ToEntity(adminId.Value);

        await _receptionistRepository.CreateReceptionistAsync(receptionist);
        await _unitOfWork.CommitAsync();

        return Result.Success(receptionist.Id);
    }
}
