using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Domain.Interfaces.Repositories;

public interface IReceptionistRepository
{
    Task<List<Receptionist>> GetAllReceptionistsAsync();
    Task<Receptionist?> GetReceptionistForUpdateAsync(int receptionistId);
    Task<User?> GetUserByReceptionistIdAsync(int receptionistId);
    Task<int?> GetAdminIdByUserIdAsync(int userId);
    Task<bool> PersonExistsAsync(int personId);
    Task<bool> ReceptionistExistsForPersonAsync(int personId);
    Task<Receptionist> CreateReceptionistAsync(Receptionist receptionist);
}
