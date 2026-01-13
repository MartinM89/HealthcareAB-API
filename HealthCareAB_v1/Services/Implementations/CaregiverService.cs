using HealthCareAB_v1.DTOs.Booking.CaregiverScheduleDtos;
using HealthCareAB_v1.Models;
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

        var scheduleDtos = schedules.Select(MapToDailyScheduleDto).ToList();

        return new CaregiverScheduleOverviewDto
        {
            CaregiverId = caregiverId,
            StartDate = startDate,
            EndDate = endDate,
            Schedules = scheduleDtos,
        }; //Return DailySchedule later instead.
    }

    public async Task<CaregiverScheduleOverviewDto> GetUpcomingSchedulesAsync(
        Guid caregiverId,
        int daysAhead = 30
    )
    {
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(daysAhead);

        return await GetScheduleOverviewAsync(caregiverId, startDate, endDate);
    }

    private static DailyScheduleDto MapToDailyScheduleDto(CaregiverDailySchedule schedule)
    {
        return new DailyScheduleDto
        {
            Id = schedule.Id,
            Start = schedule.Start,
            End = schedule.End,
            Date = DateOnly.FromDateTime(schedule.Start),
            Status = schedule.CaregiverStatus.Status,

            // Map all bookings and sort by time
            Bookings =
            [
                .. schedule.Bookings.Select(MapToBookingDto).OrderBy(b => b.TimeSlot.Start),
            ],
        };

        /* I WANT THIS.
        return new DailyScheduleDto
        {
            Id = schedule.Id,
            Start = schedule.Start,
            End = schedule.End,
            Date = DateOnly.FromDateTime(schedule.Start),
            Status = schedule.CaregiverStatus.Status,

            // Map all bookings and sort by time
            Bookings = schedule
                .Bookings.Select(MapToBookingDto)
                .OrderBy(b => b.TimeSlot.Start)
                .ToList(),
        };
         */
    }

    private static BookingDto MapToBookingDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            Comment = booking.Comment,
            CreatedAt = booking.CreatedAt,
            Date = booking.Date,
            Patient = MapToPatientInfoDto(booking.Patient),
            TimeSlot = MapToTimeSlotDto(booking.TimeSlot),
        };
    }

    private static PatientInfoDto MapToPatientInfoDto(Patient patient)
    {
        return new PatientInfoDto
        {
            Id = patient.User.Id,
            FirstName = patient.User.FirstName,
            LastName = patient.User.LastName,
            PhoneNumber = patient.User.PhoneNumber,
        };
    }

    private static TimeSlotDto MapToTimeSlotDto(TimeSlot timeSlot)
    {
        return new TimeSlotDto
        {
            Id = timeSlot.Id,
            Start = timeSlot.Start,
            End = timeSlot.End,
        };
    }
}
