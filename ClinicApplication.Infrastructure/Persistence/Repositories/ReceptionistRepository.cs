using ClinicApplication.Domain.Entities;
using ClinicApplication.Domain.Interfaces.Repositories;
using ClinicApplication.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicApplication.Infrastructure.Persistence.Repositories;

public class ReceptionistRepository : IReceptionistRepository
{
    private readonly ClinicDbContext _context;

    public ReceptionistRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Receptionist>> GetAllReceptionistsAsync()
    {
        return await _context.Receptionists
            .AsNoTracking()
            .Select(receptionist => new Receptionist
            {
                Id = receptionist.Id,
                PersonId = receptionist.PersonId,
                CreatedByAdminId = receptionist.CreatedByAdminId,
                Person = new Person
                {
                    FirstName = receptionist.Person.FirstName,
                    LastName = receptionist.Person.LastName
                },
                CreatedByAdmin = new Admin
                {
                    Person = new Person
                    {
                        FirstName = receptionist.CreatedByAdmin.Person.FirstName,
                        LastName = receptionist.CreatedByAdmin.Person.LastName
                    }
                }
            })
            .ToListAsync();
    }

    public Task<Receptionist?> GetReceptionistForUpdateAsync(int receptionistId)
        => _context.Receptionists
            .Include(receptionist => receptionist.Person)
            .FirstOrDefaultAsync(receptionist => receptionist.Id == receptionistId);

    public Task<User?> GetUserByReceptionistIdAsync(int receptionistId)
        => _context.Receptionists
            .Where(receptionist => receptionist.Id == receptionistId)
            .Join(_context.Users,
                receptionist => receptionist.PersonId,
                user => user.PersonId,
                (receptionist, user) => user)
            .FirstOrDefaultAsync();

    public Task<int?> GetAdminIdByUserIdAsync(int userId)
        => _context.Users
            .Where(user => user.Id == userId)
            .Join(_context.Admins,
                user => user.PersonId,
                admin => admin.PersonId,
                (user, admin) => (int?)admin.Id)
            .FirstOrDefaultAsync();

    public Task<bool> PersonExistsAsync(int personId)
        => _context.Persons.AnyAsync(person => person.Id == personId);

    public Task<bool> ReceptionistExistsForPersonAsync(int personId)
        => _context.Receptionists.AnyAsync(receptionist => receptionist.PersonId == personId);

    public async Task<Receptionist> CreateReceptionistAsync(Receptionist receptionist)
    {
        await _context.Receptionists.AddAsync(receptionist);
        return receptionist;
    }
}
