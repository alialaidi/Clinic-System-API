using System;
using System.Collections.Generic;

namespace ClinicApplication.Domain.Entities;

public partial class Doctor
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public int DepartmentId { get; set; }

    public string? Specialization { get; set; }

    public int CreatedByAdminId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Admin CreatedByAdmin { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;
}
