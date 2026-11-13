namespace ClinicApplication.Domain.Enums;

public enum AppointmentStatus : byte
{
    Scheduled = 0,
    Postponed = 1,
    Cancelled = 2,
    Completed = 3
}
