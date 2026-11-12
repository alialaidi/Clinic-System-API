using System;
using System.Collections.Generic;


namespace ClinicApplication.Domain.Entities;

public partial class Admin
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual Person Person { get; set; } = null!;

    public virtual ICollection<Receptionist> Receptionists { get; set; } = new List<Receptionist>();
}
