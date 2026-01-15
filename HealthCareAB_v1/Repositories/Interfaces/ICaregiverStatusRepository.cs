using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Repositories.Interfaces;

public interface ICaregiverStatusRepository
{
    Task<CaregiverStatus?> GetByIdAsync(Guid statusId);
}
