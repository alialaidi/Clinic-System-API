using System;
using System.Collections.Generic;

namespace ClinicApplication.Domain.Entities;

public partial class Receptionist
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public int CreatedByAdminId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual Admin CreatedByAdmin { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;
}
