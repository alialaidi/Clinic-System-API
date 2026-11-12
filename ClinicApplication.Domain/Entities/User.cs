using System;
using System.Collections.Generic;

namespace ClinicApplication.Domain.Entities;

public partial class User
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public byte Role { get; set; }

    public bool MustChangePassword { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Person Person { get; set; } = null!;
}
