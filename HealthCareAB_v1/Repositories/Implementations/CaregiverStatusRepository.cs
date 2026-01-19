using System.Diagnostics.CodeAnalysis;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;

namespace HealthCareAB_v1.Repositories.Implementations;

[ExcludeFromCodeCoverage]
public class CaregiverStatusRepository(AppDbContext context) : ICaregiverStatusRepository
{
    private readonly AppDbContext _context = context;

    public async Task<CaregiverStatus?> GetByIdAsync(Guid statusId)
    {
        return await _context.CaregiverStatuses.FindAsync(statusId);
    }
}
