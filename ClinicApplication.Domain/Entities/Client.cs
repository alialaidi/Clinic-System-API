using System;
using System.Collections.Generic;

namespace ClinicApplication.Domain.Entities;

public partial class Client
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public string ClientCode { get; set; } = null!;

    public int RegisteredBy { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Person Person { get; set; } = null!;

    public virtual Receptionist RegisteredByNavigation { get; set; } = null!;
}
