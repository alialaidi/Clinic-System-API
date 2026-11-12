using System;
using System.Collections.Generic;

namespace ClinicApplication.Domain.Entities;

public partial class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string Phone { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Receptionist? Receptionist { get; set; }

    public virtual User? User { get; set; }
}
