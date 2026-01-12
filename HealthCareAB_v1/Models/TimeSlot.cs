namespace HealthCareAB_v1.Models;

public class TimeSlot
{
    public Guid Id { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }

    public ICollection<Booking> Bookings { get; set; } = [];
}
