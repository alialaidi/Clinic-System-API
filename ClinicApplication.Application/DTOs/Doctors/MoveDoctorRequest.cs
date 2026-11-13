using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApplication.Application.DTOs.Doctors
{
    public class MoveDoctorRequest
    {
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than zero.")]
        public int DepartmentId { get; set; }
    }
}
