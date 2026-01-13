using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Repositories.Implementations;

public class CaregiverRepository(IAppDbContext context) : ICaregiverRepository
{
    private readonly IAppDbContext _context = context;

    public async Task CreateCaregiverAsync(Caregiver caregiver)
    {
        _context.Caregivers.Add(caregiver);
        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<CaregiverDailySchedule>> GetSchedulesWithBookingsAsync(
        Guid caregiverId,
        DateTime startDate,
        DateTime endDate
    )
    {
        var caregiversSchedule = await _context
            .CaregiverDailySchedules.Where(schedule => schedule.CaregiverUserId == caregiverId)
            .Where(schedule => schedule.Start <= endDate && schedule.End >= startDate)
            .Include(schedule => schedule.CaregiverStatus)
            .Include(schedule => schedule.Bookings)
                .ThenInclude(booking => booking.Patient)
                    .ThenInclude(patient => patient.User)
            .Include(schedule => schedule.Bookings)
                .ThenInclude(booking => booking.TimeSlot)
            .OrderBy(schedule => schedule.Start)
            .ToListAsync();

        return caregiversSchedule;
    }
}
