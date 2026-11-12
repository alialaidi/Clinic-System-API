using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Auth;
using ClinicApplication.Application.Interfaces.Services;
using ClinicApplication.Application.Mappings.Auth;
using ClinicApplication.Domain.Interfaces.Repositories;

namespace ClinicApplication.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponseDTO>> LoginAsync(LoginRequestDTO request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.Trim());
        if (user is null || !user.IsActive || !_passwordHashService.VerifyPassword(user, request.Password))
            return Result<LoginResponseDTO>.Unauthorized("Invalid email or password.");

        var role = user.Role switch
        {
            0 => "Client",
            1 => "Doctor",
            2 => "Receptionist",
            3 => "Admin",
            _ => null
        };

        if (role is null)
            return Result<LoginResponseDTO>.Unauthorized("Invalid email or password.");

        var token = _jwtTokenService.CreateToken(user.Id, user.Email, role, user.MustChangePassword);

        return Result<LoginResponseDTO>.Success(new LoginResponseDTO
        {
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc,
            UserId = user.Id,
            Email = user.Email,
            Role = role,
            MustChangePassword = user.MustChangePassword
        });
    }

    public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequestDTO request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || !user.IsActive)
            return Result.Unauthorized("The authenticated user is not available.");

        if (!_passwordHashService.VerifyPassword(user, request.CurrentPassword))
            return Result.Unauthorized("The current password is incorrect.");

        if (request.CurrentPassword == request.NewPassword)
            return Result.BadRequest("The new password must be different from the current password.");

        user.PasswordHash = _passwordHashService.HashPassword(user, request.NewPassword);
        user.MustChangePassword = false;
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }

    public async Task<Result<CurrentUserDTO>> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || !user.IsActive)
            return Result<CurrentUserDTO>.Unauthorized("The authenticated user is not available.");

        var role = user.Role switch
        {
            0 => "Client",
            1 => "Doctor",
            2 => "Receptionist",
            3 => "Admin",
            _ => null
        };

        if (role is null)
            return Result<CurrentUserDTO>.Unauthorized("The authenticated user has an invalid role.");

        return Result<CurrentUserDTO>.Success(user.ToCurrentUserDTO(role));
    }
}
