using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.DTOs.User.Caregiver;

[ExcludeFromCodeCoverage]
public class DailyScheduleDto
{
    public Guid Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateOnly Date { get; set; }
    public string Status { get; set; } = string.Empty;

    public List<BookingsForScheduleDto> Bookings { get; set; } = [];

    public int TotalBookings => Bookings.Count;
}
