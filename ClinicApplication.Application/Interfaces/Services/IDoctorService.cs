using ClinicApplication.Application.Common;
using ClinicApplication.Application.DTOs.Doctors;
using ClinicApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApplication.Application.Interfaces.Services
{
    public interface IDoctorService
    {
        Task <List<DoctorDTO>> GetAllDoctorsAsync();
        Task<DoctorDTO?> GetDoctorByIdAsync(int id);
        Task<Result> CreateDoctorAsync(CreateDoctorDTO createDoctorDTO);
        Task<Result> UpdateDoctorAsync(int doctorId, UpdateDoctorDTO updateDoctorDTO);
        Task<Result> MoveDoctorAsync(int doctorId, int newDepartmentId);    
        Task<Result> UpdateStatusAsync(int doctorId, bool isActive);
        Task<Result<List<DoctorAppointmentDTO>>> GetMyAppointmentsAsync(int userId, DateOnly? date, byte? status);
    }
}
