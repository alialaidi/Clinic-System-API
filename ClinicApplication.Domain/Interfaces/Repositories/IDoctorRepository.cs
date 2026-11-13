using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicApplication.Domain.Entities;

namespace ClinicApplication.Domain.Interfaces.Repositories
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<Doctor?> GetDoctorByIdAsync(int id);
        Task<Doctor?> UpdateDoctorAsync(Doctor doctor);
        Task<Doctor> CreateDoctorAsync(Doctor doctor);
        Task<bool> DepartmentExistsAsync(int departmentId);
        Task<User?> GetUserByDoctorIdAsync(int doctorId);
        Task <int?> GetDoctorIdByUserIdAsync(int userId);
        Task<bool> HasUpcomingAppointmentsAsync(int doctorId);
        Task<List<Appointment>> GetAppointmentsByDoctorAsync(int doctorId, DateOnly? date, byte? status);
    }


}
