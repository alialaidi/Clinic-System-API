using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Clients;
using ClinicApplication.Application.DTOs.Appointments;

namespace ClinicApplication.Application.Interfaces.Services;

public interface IClientService
{
    Task<Result<CreateClientResponseDTO>> CreateClientAsync(CreateClientDTO createClientDTO, int userId);
    Task<List<ClientDTO>> GetAllClientsAsync();
    Task<ClientDTO?> GetClientByIdAsync(int clientId);
    Task<Result<ClientDTO>> GetMyProfileAsync(int userId);
    Task<List<AppointmentDTO>> GetMyAppointmentsAsync(int userId);
    Task<Result<AppointmentDTO>> GetMyAppointmentByIdAsync(int appointmentId, int userId);
    Task<List<ClientMedicalRecordDTO>> GetMyMedicalRecordsAsync(int userId);
    Task<List<PrescriptionHistoryDTO>> GetMyPrescriptionsAsync(int userId);
    Task<ClientDTO?> GetClientByCodeAsync(string clientCode);
    Task<Result> UpdateClientAsync(int clientId, UpdateClientDTO updateClientDTO);
    Task<Result<ResetClientPasswordResponseDTO>> ResetPasswordAsync(int clientId);
    Task<Result<ClientMedicalHistoryDTO>> GetClientHistoryAsync(int clientId, int userId);
}






