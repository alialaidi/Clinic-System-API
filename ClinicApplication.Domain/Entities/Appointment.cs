using System;
using System.Collections.Generic;

namespace ClinicApplication.Domain.Entities;

public partial class Appointment
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime AppointmentDatetime { get; set; }

    public byte Status { get; set; }

    public string? PostponeReason { get; set; }

    public int? UpdatedByReceptionistId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual MedicalRecord? MedicalRecord { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Receptionist? UpdatedByReceptionist { get; set; }
}
