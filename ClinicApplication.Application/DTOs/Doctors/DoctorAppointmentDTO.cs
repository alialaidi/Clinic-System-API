using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApplication.Application.DTOs.Doctors
{
    public class DoctorAppointmentDTO
    {
        public int Id { get; set; }
        public DateTime AppointmentDatetime { get; set; }
        public byte Status { get; set; }
        public string ClientName { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
    }
}
