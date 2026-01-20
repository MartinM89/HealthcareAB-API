using HealthCareAB_v1.DTOs;
using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Services.Interfaces;

public interface ITimeSlotService
{
    Task<TimeSlot> GetByIdAsync(Guid timeSlotId);
    Task<IEnumerable<TimeSlotAvailabilityDto>> GetAvailableTimeSlotsAsync(
        DateOnly selectedDate,
        CancellationToken ct
    );
}
