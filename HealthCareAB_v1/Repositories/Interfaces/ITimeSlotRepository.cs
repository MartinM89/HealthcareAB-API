using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Repositories.Interfaces;

public interface ITimeSlotRepository
{
    Task<TimeSlot?> GetByIdAsync(Guid TimeSlotId);
    Task<ICollection<TimeSlot>> GetAllAsync(CancellationToken ct);
    Task<ICollection<CaregiverDailySchedule>> GetByDateAsync(
        DateOnly selectedDate,
        DateTime dateTime,
        CancellationToken ct
    );
}
