using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;

namespace HealthCareAB_v1.Services.Implementations;

public class CaregiverStatusService(ICaregiverStatusRepository caregiverStatusRepository)
    : ICaregiverStatusService
{
    private readonly ICaregiverStatusRepository _caregiverStatusRepository =
        caregiverStatusRepository;

    public async Task<CaregiverStatus> GetByIdAsync(Guid statusId)
    {
        if (statusId == Guid.Empty)
        {
            throw new ValidationException("Status ID cannot be empty.");
        }

        var status =
            await _caregiverStatusRepository.GetByIdAsync(statusId)
            ?? throw new NotFoundException("Caregiver status not found.");

        return status;
    }
}
