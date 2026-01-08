namespace HealthCareAB_v1.Models;

public class CaregiverDailySchedule
{
    public Guid Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public Caregiver Caregiver { get; set; } = null!;
    public CaregiverStatus CaregiverStatus { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = [];
}
