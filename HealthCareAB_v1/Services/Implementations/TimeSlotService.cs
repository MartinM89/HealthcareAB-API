using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;

namespace HealthCareAB_v1.Services.Implementations;

public class TimeSlotService(ITimeSlotRepository timeSlotRepository) : ITimeSlotService
{
    private readonly ITimeSlotRepository _timeSlotRepository = timeSlotRepository;

    public async Task<TimeSlot> GetByIdAsync(Guid timeSlotId)
    {
        return await _timeSlotRepository.GetByIdAsync(timeSlotId)
            ?? throw new NotFoundException("Time slot not found");
    }
}
