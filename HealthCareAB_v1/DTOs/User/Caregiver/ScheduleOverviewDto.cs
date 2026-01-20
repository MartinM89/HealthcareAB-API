using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.DTOs.User.Caregiver;

[ExcludeFromCodeCoverage]
public class ScheduleOverviewDto
{
    public Guid CaregiverId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public List<DailyScheduleDto> Schedules { get; set; } = [];

    public int TotalBookings => Schedules.Sum(s => s.TotalBookings);
}
