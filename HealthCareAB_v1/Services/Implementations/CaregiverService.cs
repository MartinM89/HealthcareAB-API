using HealthCareAB_v1.DTOs.User.Caregiver;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;

namespace HealthCareAB_v1.Services.Implementations;

public class CaregiverService(ICaregiverRepository caregiverRepository) : ICaregiverService
{
    private readonly ICaregiverRepository _caregiverRepository = caregiverRepository;

    public async Task<CaregiverScheduleOverviewDto> GetScheduleOverviewAsync(
        Guid caregiverId,
        DateTime startDate,
        DateTime endDate
    )
    {
        var schedules = await _caregiverRepository.GetSchedulesWithBookingsAsync(
            caregiverId,
            startDate,
            endDate
        );

        return new CaregiverScheduleOverviewDto
        {
            CaregiverId = caregiverId,
            StartDate = startDate,
            EndDate = endDate,
            Schedules =
            [
                .. schedules.Select(schedule => new DailyScheduleDto
                {
                    Id = schedule.Id,
                    Start = schedule.StartTime,
                    End = schedule.EndTime,
                    Date = DateOnly.FromDateTime(schedule.StartTime),
                    Status = schedule.CaregiverStatus.Status,
                    Bookings =
                    [
                        .. schedule
                            .Bookings.Select(booking => new BookingsForScheduleDto
                            {
                                Id = booking.Id,
                                Comment = booking.Comment,
                                CreatedAt = booking.CreatedAt,
                                Date = booking.Date,
                                Patient = new PatientInfoDto
                                {
                                    Id = booking.Patient.User.Id,
                                    FirstName = booking.Patient.User.FirstName,
                                    LastName = booking.Patient.User.LastName,
                                    PhoneNumber = booking.Patient.User.PhoneNumber,
                                },
                                TimeSlot = new TimeSlotDto
                                {
                                    Id = booking.TimeSlot.Id,
                                    Start = booking.TimeSlot.Start,
                                    End = booking.TimeSlot.End,
                                },
                            })
                            .OrderBy(b => b.TimeSlot.Start),
                    ],
                }),
            ],
        };
    }

    public async Task<CaregiverScheduleOverviewDto> GetUpcomingSchedulesAsync(
        CaregiverSchedulesDto caregiverSchedulesDto
    )
    {
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.Date.AddDays(caregiverSchedulesDto.DaysAhead);

        return await GetScheduleOverviewAsync(
            caregiverSchedulesDto.CaregiverId,
            startDate,
            endDate
        );
    }
}
