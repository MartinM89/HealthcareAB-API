using HealthCareAB_v1.DTOs.User.Caregiver;

namespace HealthCareAB_v1.Services.Interfaces;

public interface ICaregiverService
{
    Task<CaregiverScheduleOverviewDto> GetScheduleOverviewAsync(
        Guid caregiverId,
        DateTime startDate,
        DateTime endDate
    );
    Task<CaregiverScheduleOverviewDto> GetUpcomingSchedulesAsync(
        CaregiverSchedulesDto caregiverSchedulesDto
    );
}
