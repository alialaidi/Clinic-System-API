using ClinicApplication.Application.DTOs;
using ClinicApplication.Application.Common;
using ClinicApplication.Domain.Entities;
using ClinicApplication.Domain.Interfaces.Repositories;
using ClinicApplication.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicApplication.Infrastructure.Persistence.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly ClinicDbContext _context;

    public DoctorRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Doctor>> GetAllDoctorsAsync()
    {
        return await _context.Doctors
            .AsNoTracking()
            .Select(d => new Doctor
            {
                Id = d.Id,
                PersonId = d.PersonId,
                DepartmentId = d.DepartmentId,
                Specialization = d.Specialization,
                CreatedByAdminId = d.CreatedByAdminId,

                Person = new Person
                {
                    FirstName = d.Person.FirstName,
                    LastName = d.Person.LastName
                },

                Department = new Department
                {
                    Name = d.Department.Name
                },

                CreatedByAdmin = new Admin
                {
                    Person = new Person
                    {
                        FirstName = d.CreatedByAdmin.Person.FirstName,
                        LastName = d.CreatedByAdmin.Person.LastName
                    }
                }
            })
            .ToListAsync();
    }

    public async Task<Doctor?> GetDoctorByIdAsync(int id)
    {
        return await _context.Doctors
        .AsNoTracking()
        .Where(d => d.Id == id)
        .Select(d => new Doctor
        {
            Id = d.Id,
            PersonId = d.PersonId,
            DepartmentId = d.DepartmentId,
            Specialization = d.Specialization,
            CreatedByAdminId = d.CreatedByAdminId,

            Person = new Person
            {
                FirstName = d.Person.FirstName,
                LastName = d.Person.LastName
            },

            Department = new Department
            {
                Name = d.Department.Name
            },

            CreatedByAdmin = new Admin
            {
                Person = new Person
                {
                    FirstName = d.CreatedByAdmin.Person.FirstName,
                    LastName = d.CreatedByAdmin.Person.LastName
                }
            }
        })
        .FirstOrDefaultAsync();
    }

    public async Task<Doctor> CreateDoctorAsync(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return doctor;
    }

    public async Task<Doctor?> UpdateDoctorAsync(Doctor doctor)
    {
        var existingDoctor = await _context.Doctors.FindAsync(doctor.Id);
        if (existingDoctor == null)
        {
            return null;
        }
        existingDoctor.PersonId = doctor.PersonId;
        existingDoctor.DepartmentId = doctor.DepartmentId;
        existingDoctor.Specialization = doctor.Specialization;
        existingDoctor.CreatedByAdminId = doctor.CreatedByAdminId;
        await _context.SaveChangesAsync();
        return existingDoctor;
    }

    public Task<bool> DepartmentExistsAsync(int departmentId)
        => _context.Departments.AnyAsync(d => d.Id == departmentId);

    public Task<User?> GetUserByDoctorIdAsync(int doctorId)
        => _context.Doctors
            .Where(d => d.Id == doctorId)
            .Join(_context.Users,
                  d => d.PersonId,
                  u => u.PersonId,
                  (d, u) => u)
            .FirstOrDefaultAsync();

    public Task<int?> GetDoctorIdByUserIdAsync(int userId)
        => _context.Users
            .Where(u => u.Id == userId)
            .Join(_context.Doctors,
                  u => u.PersonId,
                  d => d.PersonId,
                  (u, d) => (int?)d.Id)
            .FirstOrDefaultAsync();

    public Task<bool> HasUpcomingAppointmentsAsync(int doctorId)
        => _context.Appointments.AnyAsync(a =>
            a.DoctorId == doctorId &&
            (a.Status == 0 || a.Status == 1) &&
            a.AppointmentDatetime > DateTime.UtcNow);

    public async Task<List<Appointment>> GetAppointmentsByDoctorAsync(
    int doctorId, DateOnly? date, byte? status)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
                .ThenInclude(c => c.Person)
            .Where(a => a.DoctorId == doctorId);

        if (date is not null)
        {
            var start = date.Value.ToDateTime(TimeOnly.MinValue);
            var end = start.AddDays(1);
            query = query.Where(a => a.AppointmentDatetime >= start
                                  && a.AppointmentDatetime < end);
        }

        if (status is not null)
            query = query.Where(a => a.Status == status);

        return await query
            .OrderBy(a => a.AppointmentDatetime)
            .ToListAsync();
    }

}
