using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Services.Interfaces;

public interface ICaregiverStatusService
{
    Task<CaregiverStatus> GetByIdAsync(Guid statusId);
}
