using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Domain.Interfaces.Repositories;

public interface IClientRepository
{
    Task<int?> GetReceptionistIdByUserIdAsync(int userId);
    Task<bool> UserExistsByEmailAsync(string email);
    Task<List<string>> GetClientCodesAsync();
    Task CreateClientAsync(Client client, User user);
    Task<List<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(int clientId);
    Task<Client?> GetClientByCodeAsync(string clientCode);
    Task<Client?> GetClientForUpdateAsync(int clientId);
    Task<User?> GetUserByClientIdAsync(int clientId);
    Task<List<Appointment>> GetClientHistoryAsync(int clientId, int doctorId);
}






