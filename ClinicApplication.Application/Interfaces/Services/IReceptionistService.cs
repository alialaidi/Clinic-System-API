using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Receptionists;

namespace ClinicApplication.Application.Interfaces.Services;

public interface IReceptionistService
{
    Task<Result> CreateReceptionistAsync(CreateReceptionistDTO createReceptionistDTO, int userId);
    Task<List<ReceptionistDTO>> GetAllReceptionistsAsync();
    Task<Result> UpdateReceptionistAsync(int receptionistId, UpdateReceptionistDTO updateReceptionistDTO);
    Task<Result> UpdateStatusAsync(int receptionistId, bool isActive);
}
