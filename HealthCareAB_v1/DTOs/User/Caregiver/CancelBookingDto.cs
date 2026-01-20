using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.DTOs.User.Caregiver;

[ExcludeFromCodeCoverage]
public class CancelBookingDto
{
    public Guid PatientId { get; set; }
    public Guid DailyScheduleId { get; set; }
    public Guid TimeSlotId { get; set; }
    public DateOnly Date { get; set; }
}
