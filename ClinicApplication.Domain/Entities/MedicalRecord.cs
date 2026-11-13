using System;
using System.Collections.Generic;

namespace ClinicApplication.Domain.Entities;

public partial class MedicalRecord
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public string? VisitDescription { get; set; }

    public string? Diagnosis { get; set; }

    public string? Notes { get; set; }

    public string? PatientInstructions { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
