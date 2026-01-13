using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Repositories.Interfaces;

public interface ITimeSlotRepository
{
    Task<TimeSlot?> GetById(Guid TimeSlotId);
}
