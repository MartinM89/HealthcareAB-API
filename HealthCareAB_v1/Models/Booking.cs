using System.Text.Json.Serialization;

namespace HealthCareAB_v1.Models;

public class Booking
{
    public Guid Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateOnly Date { get; set; }
    public Guid CaregiverDailyScheduleId { get; set; }
    public Guid PatientId { get; set; }
    public Guid TimeSlotId { get; set; }

    [JsonIgnore]
    public TimeSlot TimeSlot { get; set; } = null!;

    [JsonIgnore]
    public Patient Patient { get; set; } = null!;

    [JsonIgnore]
    public CaregiverDailySchedule DailySchedule { get; set; } = null!;
}
