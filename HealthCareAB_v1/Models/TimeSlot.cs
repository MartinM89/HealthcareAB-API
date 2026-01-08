namespace HealthCareAB_v1.Models;

public class TimeSlot
{
    public Guid Id { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End => SetEnd();
    public double TimeLength { get; set; } = 30;

    public ICollection<Booking> Bookings { get; set; } = [];

    public TimeOnly SetEnd() => Start.AddMinutes(TimeLength);
}
