using System.Security.Cryptography;
using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Clients;
using ClinicApplication.Application.DTOs.Appointments;
using ClinicApplication.Application.Interfaces.Services;
using ClinicApplication.Application.Mappings.Clients;
using ClinicApplication.Application.Mappings.Appointments;
using ClinicApplication.Domain.Entities;
using ClinicApplication.Domain.Interfaces.Repositories;

namespace ClinicApplication.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDoctorRepository _doctorRepository;

    public ClientService(
        IClientRepository clientRepository,
        IPasswordHashService passwordHashService,
        IUnitOfWork unitOfWork,
        IDoctorRepository doctorRepository)
    {
        _clientRepository = clientRepository;
        _passwordHashService = passwordHashService;
        _unitOfWork = unitOfWork;
        _doctorRepository = doctorRepository;
    }

    public async Task<Result<ClientMedicalHistoryDTO>> GetClientHistoryAsync(int clientId, int userId)
    {
        var doctorId = await _doctorRepository.GetDoctorIdByUserIdAsync(userId);
        if (doctorId is null)
            return Result<ClientMedicalHistoryDTO>.NotFound("The client or doctor was not found");

        var appointments = await _clientRepository.GetClientHistoryAsync(clientId, doctorId.Value);
        if (appointments.Count == 0)
            return Result<ClientMedicalHistoryDTO>.NotFound("The client or doctor was not found");

        return Result<ClientMedicalHistoryDTO>.Success(appointments.ToMedicalHistoryDTO());
    }
    public async Task<Result<ResetClientPasswordResponseDTO>> ResetPasswordAsync(int clientId)
    {
        var user = await _clientRepository.GetUserByClientIdAsync(clientId);
        if (user is null)
            return Result<ResetClientPasswordResponseDTO>.NotFound("The client or their user account was not found");

        var temporaryPassword = GenerateTemporaryPassword();
        user.PasswordHash = _passwordHashService.HashPassword(user, temporaryPassword);
        user.MustChangePassword = true;

        await _unitOfWork.CommitAsync();

        return Result<ResetClientPasswordResponseDTO>.Success(new ResetClientPasswordResponseDTO
        {
            TemporaryPassword = temporaryPassword
        });
    }
    public async Task<Result> UpdateClientAsync(int clientId, UpdateClientDTO updateClientDTO)
    {
        var client = await _clientRepository.GetClientForUpdateAsync(clientId);
        if (client is null)
            return Result.NotFound("The client was not found");

        client.Person.FirstName = updateClientDTO.FirstName;
        client.Person.LastName = updateClientDTO.LastName;
        client.Person.DateOfBirth = updateClientDTO.DateOfBirth;
        client.Person.Gender = updateClientDTO.Gender;
        client.Person.Phone = updateClientDTO.Phone;
        client.Person.Address = updateClientDTO.Address;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
    public async Task<ClientDTO?> GetClientByCodeAsync(string clientCode)
    {
        var client = await _clientRepository.GetClientByCodeAsync(clientCode);
        return client?.ToDTO();
    }
    public async Task<ClientDTO?> GetClientByIdAsync(int clientId)
    {
        var client = await _clientRepository.GetClientByIdAsync(clientId);
        return client?.ToDTO();
    }

    public async Task<Result<ClientDTO>> GetMyProfileAsync(int userId)
    {
        var client = await _clientRepository.GetClientByUserIdAsync(userId);
        if (client is null)
            return Result<ClientDTO>.NotFound("The client was not found");

        return Result<ClientDTO>.Success(client.ToDTO());
    }

    public async Task<List<AppointmentDTO>> GetMyAppointmentsAsync(int userId)
    {
        var appointments = await _clientRepository.GetAppointmentsByUserIdAsync(userId);
        return appointments.Select(appointment => appointment.ToDTO()).ToList();
    }

    public async Task<Result<AppointmentDTO>> GetMyAppointmentByIdAsync(int appointmentId, int userId)
    {
        var appointment = await _clientRepository.GetAppointmentByIdForUserAsync(appointmentId, userId);
        if (appointment is null)
            return Result<AppointmentDTO>.NotFound("The appointment was not found");

        return Result<AppointmentDTO>.Success(appointment.ToDTO());
    }

    public async Task<List<ClientMedicalRecordDTO>> GetMyMedicalRecordsAsync(int userId)
    {
        var medicalRecords = await _clientRepository.GetMedicalRecordsByUserIdAsync(userId);
        return medicalRecords.Select(medicalRecord => medicalRecord.ToClientDTO()).ToList();
    }

    public async Task<List<PrescriptionHistoryDTO>> GetMyPrescriptionsAsync(int userId)
    {
        var prescriptions = await _clientRepository.GetPrescriptionsByUserIdAsync(userId);
        return prescriptions.Select(prescription => prescription.ToDTO()).ToList();
    }
    public async Task<List<ClientDTO>> GetAllClientsAsync()
    {
        var clients = await _clientRepository.GetAllClientsAsync();
        return clients.Select(client => client.ToDTO()).ToList();
    }
    public async Task<Result<CreateClientResponseDTO>> CreateClientAsync(CreateClientDTO createClientDTO, int userId)
    {
        var receptionistId = await _clientRepository.GetReceptionistIdByUserIdAsync(userId);
        if (receptionistId is null)
            return Result<CreateClientResponseDTO>.NotFound("The authenticated receptionist was not found");

        var email = createClientDTO.Email.Trim();
        if (await _clientRepository.UserExistsByEmailAsync(email))
            return Result<CreateClientResponseDTO>.Conflict("A user with the specified email already exists");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var clientCode = GenerateClientCode(await _clientRepository.GetClientCodesAsync());
            var temporaryPassword = GenerateTemporaryPassword();
            var person = createClientDTO.ToPerson();
            var client = createClientDTO.ToEntity(clientCode, receptionistId.Value);
            client.Person = person;

            var user = new User
            {
                Person = person,
                Email = email,
                Role = 0,
                MustChangePassword = true,
                IsActive = true
            };
            user.PasswordHash = _passwordHashService.HashPassword(user, temporaryPassword);

            await _clientRepository.CreateClientAsync(client, user);
            await _unitOfWork.CommitTransactionAsync();

            return Result<CreateClientResponseDTO>.Success(new CreateClientResponseDTO
            {
                ClientCode = client.ClientCode,
                Email = user.Email,
                TemporaryPassword = temporaryPassword
            });
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private static string GenerateClientCode(IEnumerable<string> clientCodes)
    {
        var maxNumber = clientCodes
            .Where(clientCode => clientCode.StartsWith('C'))
            .Select(clientCode => long.TryParse(clientCode.AsSpan(1), out var number) ? number : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"C{checked(maxNumber + 1):D6}";
    }

    private static string GenerateTemporaryPassword()
    {
        var bytes = RandomNumberGenerator.GetBytes(12);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}







