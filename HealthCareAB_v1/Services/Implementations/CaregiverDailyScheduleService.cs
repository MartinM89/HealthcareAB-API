using HealthCareAB_v1.DTOs.Caregiver;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;

namespace HealthCareAB_v1.Services.Implementations;

public class CaregiverDailyScheduleService(
    ICaregiverStatusRepository caregiverStatusRepository,
    IUserService userService,
    ICaregiverDailyScheduleRepository caregiverDailyScheduleRepository
) : ICaregiverDailyScheduleService
{
    private readonly IUserService _userService = userService;
    private readonly ICaregiverStatusRepository _caregiverStatusRepository =
        caregiverStatusRepository;
    private readonly ICaregiverDailyScheduleRepository _caregiverDailyScheduleRepository =
        caregiverDailyScheduleRepository;

    public async Task<CaregiverDailySchedule> CreateAsync(CreateCaregiverDailyScheduleDto dto)
    {
        if (dto.Date > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
        {
            throw new ValidationException(
                "You cannot create a schedule more than 30 days in advance."
            );
        }

        var caregiver =
            await _userService.GetCaregiverByIdAsync(dto.CaregiverId)
            ?? throw new NotFoundException("Caregiver not found");

        var status =
            await _caregiverStatusRepository.GetByIdAsync(dto.CaregiverStatusId)
            ?? throw new NotFoundException("Caregiver status not found");

        var start = new TimeOnly(8, 0);
        var end = new TimeOnly(16, 0);

        var finalStartDt = dto.Date.ToDateTime(start).ToUniversalTime();
        var finalEndDt = dto.Date.ToDateTime(end).ToUniversalTime();

        var dailySchedule = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            StartTime = finalStartDt,
            EndTime = finalEndDt,
            CaregiverId = dto.CaregiverId,
            Caregiver = caregiver,
            CaregiverStatusId = dto.CaregiverStatusId,
            CaregiverStatus = status,
        };

        return await _caregiverDailyScheduleRepository.CreateAsync(dailySchedule);
    }
}
