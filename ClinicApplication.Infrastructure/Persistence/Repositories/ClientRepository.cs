using ClinicApplication.Domain.Entities;
using ClinicApplication.Domain.Interfaces.Repositories;
using ClinicApplication.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicApplication.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ClinicDbContext _context;

    public ClientRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public Task<int?> GetReceptionistIdByUserIdAsync(int userId)
        => _context.Users
            .Where(user => user.Id == userId)
            .Join(_context.Receptionists,
                user => user.PersonId,
                receptionist => receptionist.PersonId,
                (user, receptionist) => (int?)receptionist.Id)
            .FirstOrDefaultAsync();

    public Task<bool> UserExistsByEmailAsync(string email)
        => _context.Users.AnyAsync(user => user.Email == email);

    public Task<List<string>> GetClientCodesAsync()
        => _context.Clients
            .AsNoTracking()
            .Select(client => client.ClientCode)
            .ToListAsync();

    public Task<List<Client>> GetAllClientsAsync()
        => _context.Clients
            .AsNoTracking()
            .Select(client => new Client
            {
                Id = client.Id,
                ClientCode = client.ClientCode,
                Person = new Person
                {
                    FirstName = client.Person.FirstName,
                    LastName = client.Person.LastName,
                    DateOfBirth = client.Person.DateOfBirth,
                    Gender = client.Person.Gender,
                    Phone = client.Person.Phone,
                    Address = client.Person.Address,
                    User = client.Person.User == null
                        ? null
                        : new User { Email = client.Person.User.Email }
                }
            })
            .ToListAsync();
    public Task<Client?> GetClientByIdAsync(int clientId)
        => _context.Clients
            .AsNoTracking()
            .Where(client => client.Id == clientId)
            .Select(client => new Client
            {
                Id = client.Id,
                ClientCode = client.ClientCode,
                Person = new Person
                {
                    FirstName = client.Person.FirstName,
                    LastName = client.Person.LastName,
                    DateOfBirth = client.Person.DateOfBirth,
                    Gender = client.Person.Gender,
                    Phone = client.Person.Phone,
                    Address = client.Person.Address,
                    User = client.Person.User == null
                        ? null
                        : new User { Email = client.Person.User.Email }
                }
            })
            .FirstOrDefaultAsync();
    public Task<Client?> GetClientByCodeAsync(string clientCode)
        => _context.Clients
            .AsNoTracking()
            .Where(client => client.ClientCode == clientCode)
            .Select(client => new Client
            {
                Id = client.Id,
                ClientCode = client.ClientCode,
                Person = new Person
                {
                    FirstName = client.Person.FirstName,
                    LastName = client.Person.LastName,
                    DateOfBirth = client.Person.DateOfBirth,
                    Gender = client.Person.Gender,
                    Phone = client.Person.Phone,
                    Address = client.Person.Address,
                    User = client.Person.User == null
                        ? null
                        : new User { Email = client.Person.User.Email }
                }
            })
            .FirstOrDefaultAsync();
    public Task<List<Appointment>> GetClientHistoryAsync(int clientId, int doctorId)
        => _context.Appointments
            .AsNoTracking()
            .Where(appointment => appointment.ClientId == clientId &&
                appointment.Client.Appointments.Any(doctorAppointment => doctorAppointment.DoctorId == doctorId))
            .Include(appointment => appointment.Client)
                .ThenInclude(client => client.Person)
            .Include(appointment => appointment.Doctor)
                .ThenInclude(doctor => doctor.Person)
            .Include(appointment => appointment.MedicalRecord)
                .ThenInclude(medicalRecord => medicalRecord!.Prescriptions)
                    .ThenInclude(prescription => prescription.PrescriptionItems)
            .AsSplitQuery()
            .OrderByDescending(appointment => appointment.AppointmentDatetime)
            .ToListAsync();
    public Task<User?> GetUserByClientIdAsync(int clientId)
        => _context.Clients
            .Where(client => client.Id == clientId)
            .Join(_context.Users,
                client => client.PersonId,
                user => user.PersonId,
                (client, user) => user)
            .FirstOrDefaultAsync();
    public Task<Client?> GetClientForUpdateAsync(int clientId)
        => _context.Clients
            .Include(client => client.Person)
            .FirstOrDefaultAsync(client => client.Id == clientId);
    public async Task CreateClientAsync(Client client, User user)
    {
        await _context.Clients.AddAsync(client);
        await _context.Users.AddAsync(user);
    }
}






