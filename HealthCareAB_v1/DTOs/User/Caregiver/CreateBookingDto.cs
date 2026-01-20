using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.DTOs.User.Caregiver;

[ExcludeFromCodeCoverage]
public class CreateBookingDto
{
    public Guid PatientId { get; set; }
    public Guid CaregiverDailyScheduleId { get; set; }
    public Guid TimeSlotId { get; set; }
    public DateOnly Date { get; set; }
    public string Comment { get; set; } = string.Empty;
}
