namespace HealthCareAB_v1.Models;

public class Booking
{
    public Guid Id { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateOnly Date { get; set; }

    public TimeSlot TimeSlot { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public CaregiverDailySchedule DailySchedule { get; set; } = null!;
}
