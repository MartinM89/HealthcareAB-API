namespace HealthCareAB_v1.Models;

public class CaregiverDailySchedule
{
    public Guid Id { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public Guid CaregiverId { get; set; }
    public Caregiver Caregiver { get; set; } = null!;

    public Guid CaregiverStatusId { get; set; }
    public CaregiverStatus CaregiverStatus { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = [];
}
