using HealthCareAB_v1.DTOs.User.Caregiver;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;

namespace HealthCareAB_v1.Services.Implementations;

public class CaregiverService(ICaregiverRepository caregiverRepository) : ICaregiverService
{
    private readonly ICaregiverRepository _caregiverRepository = caregiverRepository;

    public async Task<ScheduleOverviewDto> GetScheduleOverviewAsync(
        Guid caregiverId,
        DateTime startDate,
        DateTime endDate
    )
    {
        if (caregiverId == Guid.Empty)
        {
            throw new ArgumentException("Caregiver ID cannot be empty");
        }

        if (endDate < startDate)
        {
            throw new ArgumentException("End date cannot be before start date");
        }

        if ((endDate - startDate).TotalDays > 30)
        {
            throw new ArgumentException("Date range cannot exceed 30 days");
        }

        var schedules =
            await _caregiverRepository.GetSchedulesWithBookingsAsync(
                caregiverId,
                startDate,
                endDate
            ) ?? throw new NotFoundException($"Caregiver with ID {caregiverId} not found");

        return new ScheduleOverviewDto
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

    public async Task<ScheduleOverviewDto> GetUpcomingSchedulesAsync(
        Guid caregiverId,
        int daysAhead = 30
    )
    {
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.Date.AddDays(daysAhead);

        return await GetScheduleOverviewAsync(caregiverId, startDate, endDate);
    }

    public async Task<Booking> CreateBookingForPatientAsync(
        Guid caregiverId,
        CreateBookingDto request
    )
    {
        var patient =
            await _caregiverRepository.GetPatientByIdAsync(request)
            ?? throw new NotFoundException($"Patient with ID {request.PatientId} not found");

        var dailySchedule =
            await _caregiverRepository.GetCaregiversDailyScheduleAsync(request)
            ?? throw new NotFoundException(
                $"Schedule with ID {request.CaregiverDailyScheduleId} not found"
            );

        if (dailySchedule.CaregiverId != caregiverId)
        {
            throw new UnauthorizedAccessException(
                "You can only create bookings on your own schedules"
            );
        }

        if (dailySchedule.CaregiverStatus.Status != CaregiverStatuses.Available)
        {
            throw new InvalidOperationException(
                $"Cannot create booking - schedule status is {dailySchedule.CaregiverStatus.Status}"
            );
        }

        var timeSlot =
            await _caregiverRepository.GetTimeSlotAsync(request)
            ?? throw new NotFoundException($"Time slot with ID {request.TimeSlotId} not found");

        var slotDateTime = request.Date.ToDateTime(timeSlot.Start).ToUniversalTime();

        if (slotDateTime < dailySchedule.StartTime || slotDateTime >= dailySchedule.EndTime)
        {
            throw new InvalidOperationException(
                $"Time slot {timeSlot.Start}-{timeSlot.End} is outside schedule working hours "
                    + $"({dailySchedule.StartTime:HH:mm}-{dailySchedule.EndTime:HH:mm} UTC)"
            );
        }

        var isSlotBooked = dailySchedule.Bookings.Any(b =>
            b.TimeSlotId == request.TimeSlotId && b.Date == request.Date
        );

        if (isSlotBooked)
        {
            throw new InvalidOperationException(
                $"Time slot {timeSlot.Start}-{timeSlot.End} is already booked on {request.Date}"
            );
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Comment = request.Comment,
            Date = request.Date,
            PatientId = request.PatientId,
            TimeSlotId = request.TimeSlotId,
            CaregiverDailyScheduleId = request.CaregiverDailyScheduleId,
            CreatedAt = DateTime.UtcNow,
        };

        await _caregiverRepository.AddBookingAsync(booking);

        return booking;
    }

    public async Task<Booking> CancelBookingForPatientAsync(
        Guid caregiverId,
        CancelBookingDto request
    )
    {
        if (caregiverId == Guid.Empty)
        {
            throw new ArgumentException("Caregiver ID cannot be empty");
        }

        var booking =
            await _caregiverRepository.GetBookingAsync(request)
            ?? throw new NotFoundException($"Booking for canceling not found");

        if (booking.DailySchedule.CaregiverId != caregiverId)
        {
            throw new UnauthorizedAccessException(
                "You can only cancel bookings on your own schedules"
            );
        }

        await _caregiverRepository.RemoveBookingAsync(booking);

        return booking;
    }
}
