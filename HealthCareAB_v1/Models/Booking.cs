using System.Text.Json.Serialization;

namespace HealthCareAB_v1.Models;

public class Booking
{
    public Guid Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateOnly Date { get; set; }

    // public string UserId { get; set; } = string.Empty;

    [JsonIgnore]
    public TimeSlot TimeSlot { get; set; } = null!;

    [JsonIgnore]
    public Patient Patient { get; set; } = null!;
    // public CaregiverDailySchedule DailySchedule { get; set; } = null!;
    public Guid CaregiverDailyScheduleId { get; set; }
    public CaregiverDailySchedule DailySchedule { get; set; } = null!;

}
