using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Application.Interfaces.Services;

public interface IPasswordHashService
{
    bool VerifyPassword(User user, string password);
    string HashPassword(User user, string password);
}
