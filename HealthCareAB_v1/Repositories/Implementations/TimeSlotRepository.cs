using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Repositories.Implementations;

public class TimeSlotRepository(IAppDbContext context) : ITimeSlotRepository
{
    private readonly IAppDbContext _context = context;

    public async Task<TimeSlot?> GetById(Guid TimeSlotId)
    {
        return await _context.TimeSlots.FirstOrDefaultAsync(t => t.Id == TimeSlotId);
    }
}
