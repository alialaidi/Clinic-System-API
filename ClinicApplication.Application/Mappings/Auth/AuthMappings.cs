using ClinicApplication.Application.DTOs.Auth;
using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Application.Mappings.Auth;

public static class AuthMappings
{
    public static CurrentUserDTO ToCurrentUserDTO(this User user, string role)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new CurrentUserDTO
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.Person.FirstName,
            LastName = user.Person.LastName,
            Role = role,
            MustChangePassword = user.MustChangePassword
        };
    }
}
