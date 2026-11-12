using ClinicApplication.Application.Interfaces.Services;
using ClinicApplication.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ClinicApplication.API.Services;

public class AspNetIdentityPasswordHashService : IPasswordHashService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public bool VerifyPassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }

    public string HashPassword(User user, string password)
        => _passwordHasher.HashPassword(user, password);
}
