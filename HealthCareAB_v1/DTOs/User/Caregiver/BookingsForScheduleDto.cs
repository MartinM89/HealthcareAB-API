using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.DTOs.User.Caregiver;

[ExcludeFromCodeCoverage]
public class BookingsForScheduleDto
{
    public Guid Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateOnly Date { get; set; }

    public PatientInfoDto Patient { get; set; } = null!;

    public TimeSlotDto TimeSlot { get; set; } = null!;
}
