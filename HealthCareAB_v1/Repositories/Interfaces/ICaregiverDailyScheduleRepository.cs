using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Repositories.Interfaces;

public interface ICaregiverDailyScheduleRepository
{
    Task<CaregiverDailySchedule> CreateAsync(CaregiverDailySchedule caregiverDailySchedule);
    Task<CaregiverDailySchedule?> GetByIdAsync(Guid dailyScheduleId);
}
