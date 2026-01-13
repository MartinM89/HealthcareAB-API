using HealthCareAB_v1.DTOs.Caregiver;
using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Services.Interfaces;

public interface ICaregiverDailyScheduleService
{
    Task<CaregiverDailySchedule> CreateAsync(CreateCaregiverDailyScheduleDto dto);
}
