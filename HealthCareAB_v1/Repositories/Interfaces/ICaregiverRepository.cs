using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Repositories.Interfaces;

public interface ICaregiverRepository
{
    Task CreateCaregiverAsync(Caregiver caregiver);
    Task<ICollection<CaregiverDailySchedule>> GetSchedulesWithBookingsAsync(
        Guid caregiverId,
        DateTime startDate,
        DateTime endDate
    );
}
