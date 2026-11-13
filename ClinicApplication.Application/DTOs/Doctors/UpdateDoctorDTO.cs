using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApplication.Application.DTOs.Doctors
{
    public class UpdateDoctorDTO
    {
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "PersonId must be greater than zero.")]
        public int PersonId { get; set; }

        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than zero.")]
        public int DepartmentId { get; set; } 

        [System.ComponentModel.DataAnnotations.StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
        public string? Specialization { get; set; }
    }
}
