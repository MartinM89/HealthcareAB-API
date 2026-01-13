using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;

namespace HealthCareAB_v1.Services.Implementations;

public class TimeSlotService(ITimeSlotRepository repository) : ITimeSlotService
{
    private readonly ITimeSlotRepository _repository = repository;

    public async Task<TimeSlot> GetById(Guid timeSlotId)
    {
        return await _repository.GetById(timeSlotId)
            ?? throw new NotFoundException("Time slot not found");
    }
}
