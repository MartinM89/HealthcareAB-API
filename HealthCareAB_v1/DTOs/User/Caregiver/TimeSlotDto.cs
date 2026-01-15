namespace HealthCareAB_v1.DTOs.User.Caregiver;

public class TimeSlotDto
{
    public Guid Id { get; set; }
    public TimeOnly Start { get; set; } // e.g., 08:00
    public TimeOnly End { get; set; } // e.g., 08:30

    /// <summary>
    /// Formatted time range for display (e.g., "08:00-08:30")
    /// </summary>
    public string TimeRange => $"{Start:HH:mm}-{End:HH:mm}";
}
