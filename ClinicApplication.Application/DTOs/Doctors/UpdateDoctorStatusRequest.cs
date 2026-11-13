using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApplication.Application.DTOs.Doctors
{
    public class UpdateDoctorStatusRequest
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "IsActive is required.")]
        public bool? IsActive { get; set; }
    }
}
