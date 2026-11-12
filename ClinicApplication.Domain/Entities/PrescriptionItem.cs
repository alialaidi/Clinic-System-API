using System;
using System.Collections.Generic;

namespace ClinicApplication.Domain.Entities;

public partial class PrescriptionItem
{
    public int Id { get; set; }

    public int PrescriptionId { get; set; }

    public string MedicationName { get; set; } = null!;

    public string Dosage { get; set; } = null!;

    public string Frequency { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? SpecialInstructions { get; set; }

    public virtual Prescription Prescription { get; set; } = null!;
}
