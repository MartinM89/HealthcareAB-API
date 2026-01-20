using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.DTOs.User.Caregiver;

[ExcludeFromCodeCoverage]
public class TimeSlotDto
{
    public Guid Id { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public string TimeRange => $"{Start:HH:mm}-{End:HH:mm}";
}
