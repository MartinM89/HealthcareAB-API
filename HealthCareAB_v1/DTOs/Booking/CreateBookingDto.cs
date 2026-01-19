namespace HealthCareAB_v1.DTOs.Booking;

public class CreateBookingDto
{
    public string Comment { get; set; } = string.Empty;
    public TimeOnly Start { get; set; }
    public DateOnly Date { get; set; }
    public Guid CaregiverDailyScheduleId { get; set; }
}
