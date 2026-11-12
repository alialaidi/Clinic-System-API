using System;
using System.Collections.Generic;

namespace ClinicApplication.Domain.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string? Notes { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
}
