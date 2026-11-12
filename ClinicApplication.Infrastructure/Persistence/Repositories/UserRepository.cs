using ClinicApplication.Domain.Entities;
using ClinicApplication.Domain.Interfaces.Repositories;
using ClinicApplication.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicApplication.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ClinicDbContext _context;

    public UserRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByEmailAsync(string email)
        => _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email);

    public Task<User?> GetByIdAsync(int userId)
        => _context.Users
            .Include(user => user.Person)
            .FirstOrDefaultAsync(user => user.Id == userId);
}
