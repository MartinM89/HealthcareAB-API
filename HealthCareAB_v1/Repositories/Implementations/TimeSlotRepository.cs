using System.Diagnostics.CodeAnalysis;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Repositories.Implementations;

[ExcludeFromCodeCoverage]
public class TimeSlotRepository(IAppDbContext context) : ITimeSlotRepository
{
    private readonly IAppDbContext _context = context;

    public async Task<TimeSlot?> GetByIdAsync(Guid TimeSlotId)
    {
        return await _context.TimeSlots.FirstOrDefaultAsync(t => t.Id == TimeSlotId);
    }
}
