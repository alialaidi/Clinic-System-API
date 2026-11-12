namespace ClinicApplication.Application.Interfaces.Services;

public interface IJwtTokenService
{
    (string AccessToken, DateTime ExpiresAtUtc) CreateToken(int userId, string email, string role, bool mustChangePassword);
}
