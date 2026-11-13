using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Appointments;
using ClinicApplication.Application.DTOs.MedicalRecords;
using ClinicApplication.Application.DTOs.Payments;
using ClinicApplication.Application.Interfaces.Services;
using ClinicApplication.Application.Mappings.Appointments;
using ClinicApplication.Application.Mappings.MedicalRecords;
using ClinicApplication.Application.Mappings.Payments;
using ClinicApplication.Domain.Entities;
using ClinicApplication.Domain.Enums;
using ClinicApplication.Domain.Interfaces.Repositories;

namespace ClinicApplication.Application.Services;

public class AppointmentService : IAppointmentService
{
    private static readonly TimeZoneInfo JordanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Amman");

    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDoctorRepository _doctorRepository;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork,
        IDoctorRepository doctorRepository)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
        _doctorRepository = doctorRepository;
    }

    public async Task<Result> CreateAppointmentAsync(CreateAppointmentDTO createAppointmentDTO, int userId)
    {
        if (!IsFutureAppointmentTime(createAppointmentDTO.AppointmentDatetime))
            return Result.BadRequest("Appointment date and time must be in the future according to Asia/Amman time.");

        if (!await _appointmentRepository.ClientExistsAsync(createAppointmentDTO.ClientId))
            return Result.NotFound("The specified client was not found");

        if (!await _appointmentRepository.DoctorExistsAsync(createAppointmentDTO.DoctorId))
            return Result.NotFound("The specified doctor was not found");

        var doctorUser = await _doctorRepository.GetUserByDoctorIdAsync(createAppointmentDTO.DoctorId);
        if (doctorUser is null)
            return Result.NotFound("The doctor account was not found");

        if (!doctorUser.IsActive)
            return Result.Conflict("The specified doctor is inactive and cannot receive appointments.");

        if (!await _appointmentRepository.IsDoctorSlotAvailableAsync(
                createAppointmentDTO.DoctorId, createAppointmentDTO.AppointmentDatetime))
            return Result.Conflict("The doctor already has an appointment at the specified date and time.");

        var receptionistId = await _appointmentRepository.GetReceptionistIdByUserIdAsync(userId);
        if (receptionistId is null)
            return Result.NotFound("The receptionist was not found");

        var appointment = createAppointmentDTO.ToEntity(receptionistId.Value);
        await _appointmentRepository.CreateAppointmentAsync(appointment);
        await _unitOfWork.CommitAsync();

        return Result.Success(appointment.Id);
    }

    public async Task<List<AppointmentDTO>> GetAllAppointmentsAsync()
    {
        var appointments = await _appointmentRepository.GetAllAppointmentsAsync();
        return appointments.Select(appointment => appointment.ToDTO()).ToList();
    }

    public async Task<Result<AppointmentDTO>> GetAppointmentByIdAsync(int appointmentId, int userId, bool isReceptionist)
    {
        Appointment? appointment;

        if (isReceptionist)
        {
            appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
        }
        else
        {
            var doctorId = await _doctorRepository.GetDoctorIdByUserIdAsync(userId);
            if (doctorId is null)
                return Result<AppointmentDTO>.NotFound("The doctor was not found");

            appointment = await _appointmentRepository.GetAppointmentByIdForDoctorAsync(appointmentId, doctorId.Value);
        }

        if (appointment is null)
            return Result<AppointmentDTO>.NotFound("The appointment was not found");

        return Result<AppointmentDTO>.Success(appointment.ToDTO());
    }

    public async Task<Result> PostponeAppointmentAsync(int appointmentId, PostponeAppointmentDTO request, int userId)
    {
        if (!IsFutureAppointmentTime(request.AppointmentDatetime))
            return Result.BadRequest("Appointment date and time must be in the future according to Asia/Amman time.");

        var appointment = await _appointmentRepository.GetAppointmentForUpdateAsync(appointmentId);
        if (appointment is null)
            return Result.NotFound("The appointment was not found");

        if (!IsScheduledOrPostponed(appointment.Status))
            return Result.Conflict("Only scheduled or postponed appointments can be postponed.");

        if (!await _appointmentRepository.IsDoctorSlotAvailableAsync(
                appointment.DoctorId, request.AppointmentDatetime, appointment.Id))
            return Result.Conflict("The doctor already has an appointment at the specified date and time.");

        var receptionistId = await _appointmentRepository.GetReceptionistIdByUserIdAsync(userId);
        if (receptionistId is null)
            return Result.NotFound("The receptionist was not found");

        appointment.AppointmentDatetime = request.AppointmentDatetime;
        appointment.PostponeReason = request.PostponeReason;
        appointment.Status = (byte)AppointmentStatus.Postponed;
        appointment.UpdatedAt = DateTime.UtcNow;
        appointment.UpdatedByReceptionistId = receptionistId.Value;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }

    public async Task<Result> CancelAppointmentAsync(int appointmentId, int userId)
    {
        var appointment = await _appointmentRepository.GetAppointmentForUpdateAsync(appointmentId);
        if (appointment is null)
            return Result.NotFound("The appointment was not found");

        if (!IsScheduledOrPostponed(appointment.Status))
            return Result.Conflict("Only scheduled or postponed appointments can be cancelled.");

        var receptionistId = await _appointmentRepository.GetReceptionistIdByUserIdAsync(userId);
        if (receptionistId is null)
            return Result.NotFound("The receptionist was not found");

        appointment.Status = (byte)AppointmentStatus.Cancelled;
        appointment.UpdatedAt = DateTime.UtcNow;
        appointment.UpdatedByReceptionistId = receptionistId.Value;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }

    public async Task<Result> CompleteAppointmentAsync(int appointmentId, int userId)
    {
        var doctorId = await _doctorRepository.GetDoctorIdByUserIdAsync(userId);
        if (doctorId is null)
            return Result.NotFound("The doctor was not found");

        var appointment = await _appointmentRepository.GetAppointmentForUpdateByDoctorAsync(appointmentId, doctorId.Value);
        if (appointment is null)
            return Result.NotFound("The appointment was not found");

        if (!IsScheduledOrPostponed(appointment.Status))
            return Result.Conflict("Only scheduled or postponed appointments can be completed.");

        appointment.Status = (byte)AppointmentStatus.Completed;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }

    public async Task<Result> CreateMedicalRecordAsync(
        int appointmentId,
        CreateMedicalRecordDTO createMedicalRecordDTO,
        int userId)
    {
        var doctorId = await _doctorRepository.GetDoctorIdByUserIdAsync(userId);
        if (doctorId is null)
            return Result.NotFound("The doctor was not found");

        var appointment = await _appointmentRepository.GetAppointmentForMedicalRecordAsync(appointmentId, doctorId.Value);
        if (appointment is null)
            return Result.NotFound("The appointment was not found");

        if (appointment.MedicalRecord is not null)
            return Result.Conflict("A medical record already exists for this appointment.");

        var medicalRecord = createMedicalRecordDTO.ToEntity(appointment.Id);
        await _appointmentRepository.CreateMedicalRecordAsync(medicalRecord);
        await _unitOfWork.CommitAsync();

        return Result.Success(medicalRecord.Id);
    }

    public async Task<Result> CreatePaymentAsync(int appointmentId, CreatePaymentDTO createPaymentDTO)
    {
        if (!await _appointmentRepository.AppointmentExistsAsync(appointmentId))
            return Result.NotFound("The appointment was not found");

        var payment = createPaymentDTO.ToEntity(appointmentId);
        await _appointmentRepository.CreatePaymentAsync(payment);
        await _unitOfWork.CommitAsync();

        return Result.Success(payment.Id);
    }

    public async Task<Result<List<PaymentDTO>>> GetPaymentsByAppointmentIdAsync(int appointmentId)
    {
        if (!await _appointmentRepository.AppointmentExistsAsync(appointmentId))
            return Result<List<PaymentDTO>>.NotFound("The appointment was not found");

        var payments = await _appointmentRepository.GetPaymentsByAppointmentIdAsync(appointmentId);
        return Result<List<PaymentDTO>>.Success(payments.Select(payment => payment.ToDTO()).ToList());
    }

    public async Task<List<PaymentDTO>> GetAllPaymentsAsync()
    {
        var payments = await _appointmentRepository.GetAllPaymentsAsync();
        return payments.Select(payment => payment.ToDTO()).ToList();
    }

    private static bool IsScheduledOrPostponed(byte status)
        => status is (byte)AppointmentStatus.Scheduled or (byte)AppointmentStatus.Postponed;

    private static bool IsFutureAppointmentTime(DateTime appointmentDatetime)
    {
        var jordanLocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, JordanTimeZone);
        var requestedLocalTime = DateTime.SpecifyKind(appointmentDatetime, DateTimeKind.Unspecified);
        return requestedLocalTime > jordanLocalNow;
    }
}
