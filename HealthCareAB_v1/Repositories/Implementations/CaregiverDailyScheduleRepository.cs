using System.Diagnostics.CodeAnalysis;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;

namespace HealthCareAB_v1.Repositories.Implementations;

[ExcludeFromCodeCoverage]
public class CaregiverDailyScheduleRepository(AppDbContext context)
    : ICaregiverDailyScheduleRepository
{
    private readonly AppDbContext _context = context;

    public async Task<CaregiverDailySchedule> CreateAsync(
        CaregiverDailySchedule caregiverDailySchedule
    )
    {
        _context.CaregiverDailySchedules.Add(caregiverDailySchedule);
        await _context.SaveChangesAsync();
        return caregiverDailySchedule;
    }
}
