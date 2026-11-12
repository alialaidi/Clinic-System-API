using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int userId);
}
