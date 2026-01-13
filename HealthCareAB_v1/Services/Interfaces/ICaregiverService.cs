using HealthCareAB_v1.DTOs.Booking.CaregiverScheduleDtos;

namespace HealthCareAB_v1.Services.Interfaces;

public interface ICaregiverService
{
    Task<CaregiverScheduleOverviewDto> GetScheduleOverviewAsync(
        Guid caregiverId,
        DateTime startDate,
        DateTime endDate
    );
    Task<CaregiverScheduleOverviewDto> GetUpcomingSchedulesAsync(
        Guid caregiverId,
        int daysAhead = 30
    );
}
