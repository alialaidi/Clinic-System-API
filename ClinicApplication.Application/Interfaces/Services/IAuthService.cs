using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Auth;

namespace ClinicApplication.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO request);
    Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequestDTO request);
    Task<Result<CurrentUserDTO>> GetCurrentUserAsync(int userId);
}
