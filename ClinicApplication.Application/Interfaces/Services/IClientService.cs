using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Clients;

namespace ClinicApplication.Application.Interfaces.Services;

public interface IClientService
{
    Task<Result<CreateClientResponseDTO>> CreateClientAsync(CreateClientDTO createClientDTO, int userId);
    Task<List<ClientDTO>> GetAllClientsAsync();
    Task<ClientDTO?> GetClientByIdAsync(int clientId);
    Task<ClientDTO?> GetClientByCodeAsync(string clientCode);
    Task<Result> UpdateClientAsync(int clientId, UpdateClientDTO updateClientDTO);
    Task<Result<ResetClientPasswordResponseDTO>> ResetPasswordAsync(int clientId);
    Task<Result<ClientMedicalHistoryDTO>> GetClientHistoryAsync(int clientId, int userId);
}






