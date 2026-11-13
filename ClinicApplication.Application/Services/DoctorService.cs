using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Doctors;
using ClinicApplication.Application.Interfaces.Services;
using ClinicApplication.Application.Mappings.Doctors;
using ClinicApplication.Domain.Entities;
using ClinicApplication.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApplication.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DoctorService(IDoctorRepository doctorRepository, IUnitOfWork uow)
        {
            _doctorRepository = doctorRepository;
            _unitOfWork = uow;
        }

        public async Task<List<DoctorDTO>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetAllDoctorsAsync();
            return doctors.Select(d => d.ToDTO()).ToList();
        }

        public async Task<DoctorDTO?> GetDoctorByIdAsync(int id)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
            return doctor?.ToDTO();
        }
       

        public async Task<Result> CreateDoctorAsync(CreateDoctorDTO createDoctorDTO)
        {
            if (!await _doctorRepository.DepartmentExistsAsync(createDoctorDTO.DepartmentId))
                return Result.NotFound("The specified department was not found");
            var doctor = createDoctorDTO.ToEntity();
            await _doctorRepository.CreateDoctorAsync(doctor);
            await _unitOfWork.CommitAsync();
            return Result.Success(doctor.Id);
        }


        public async Task<Result> UpdateDoctorAsync(int doctorId, UpdateDoctorDTO updateDoctorDTO)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor is null)
                return Result.NotFound("The doctor was not found");
            
            if (!await _doctorRepository.DepartmentExistsAsync(updateDoctorDTO.DepartmentId))
                return Result.NotFound("The specified department was not found");

            doctor.DepartmentId = updateDoctorDTO.DepartmentId;
            doctor.Specialization = updateDoctorDTO.Specialization;
            await _unitOfWork.CommitAsync();
            return Result.Success();
        }

        public async Task<Result> MoveDoctorAsync(int doctorId, int newDepartmentId)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor is null)
                return Result.NotFound("The doctor was not found");

            if (!await _doctorRepository.DepartmentExistsAsync(newDepartmentId))
                return Result.NotFound("The new department was not found");

            if (doctor.DepartmentId == newDepartmentId)
                return Result.BadRequest("The doctor is already in the specified department");

            doctor.DepartmentId = newDepartmentId;   
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }

        public async Task<Result> UpdateStatusAsync(int doctorId, bool isActive)
        {
            var user = await _doctorRepository.GetUserByDoctorIdAsync(doctorId);
            if (user is null)
                return Result.NotFound("The doctor was not found");

            if (user.IsActive == isActive)
                return Result.BadRequest(isActive ? "The doctor is already active" : "The doctor is already inactive");

            if (!isActive && await _doctorRepository.HasUpcomingAppointmentsAsync(doctorId))
                return Result.Conflict("The doctor has upcoming appointments, please handle them first");

            user.IsActive = isActive;              
            await _unitOfWork.CommitAsync();       

            return Result.Success();
        }

        public async Task<Result<List<DoctorAppointmentDTO>>> GetMyAppointmentsAsync(
            int userId, DateOnly? date, byte? status)
        {
            var doctorId = await _doctorRepository.GetDoctorIdByUserIdAsync(userId);
            if (doctorId is null)
                return Result<List<DoctorAppointmentDTO>>.NotFound("The doctor was not found");

            var appointments = await _doctorRepository.GetAppointmentsByDoctorAsync(doctorId.Value, date, status);

            var dtos = appointments.Select(a => new DoctorAppointmentDTO
            {
                Id = a.Id,
                AppointmentDatetime = a.AppointmentDatetime,
                Status = a.Status,
                ClientName = $"{a.Client.Person.FirstName} {a.Client.Person.LastName}",
                ClientCode = a.Client.ClientCode
            }).ToList();

            return Result<List<DoctorAppointmentDTO>>.Success(dtos);
        }
    
    
    }
}
