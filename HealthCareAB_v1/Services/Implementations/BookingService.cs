using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;
using HealthCareAB_v1.Services.Results;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Services.Implementations;

public class BookingService(
    IBookingRepository bookingRepository,
    ITimeSlotService timeSlotService,
    ICaregiverDailyScheduleService caregiverDailyScheduleService,
    IUserService userService
) : IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly ITimeSlotService _timeSlotService = timeSlotService;
    private readonly ICaregiverDailyScheduleService _caregiverDailyScheduleService =
        caregiverDailyScheduleService;

    private readonly IUserService _userService = userService;

    public async Task<Booking> CreateAsync(string userId, CreateBookingDto dto)
    {
        var patient =
            await _userService.GetPatientByIdAsync(Guid.Parse(userId))
            ?? throw new NotFoundException("Patient not found.");

        var timeslot = await _timeSlotService.GetByIdAsync(dto.TimeSlotId);

        var schedule = await _caregiverDailyScheduleService.GetByIdAsync(dto.ScheduleId);

        if (schedule.Bookings.ToList().Any(b => b.TimeSlot?.Id == timeslot.Id))
        {
            throw new ValidationException("This time is already booked");
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Comment = dto.Comment ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            Date = dto.Date,
            TimeSlot = timeslot,
            Patient = patient,
            DailySchedule = schedule,
        };

        return await _bookingRepository.CreateAsync(booking);
    }

    public async Task<CancelBookingResult> CancelAsync(
        Guid bookingId,
        Guid patientId,
        CancellationToken ct
    )
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, ct);

        if (booking == null)
        {
            return CancelBookingResult.BookingDoesNotExist;
        }

        if (booking.Patient == null || booking.Patient.UserId != patientId)
        {
            return CancelBookingResult.NotOwnedByPatient;
        }

        await _bookingRepository.DeleteAsync(booking, ct);
        return CancelBookingResult.Cancelled;
    }

    public async Task<List<BookingResponseDto>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken ct
    )
    {
        var bookings = await _bookingRepository.GetByPatientIdAsync(patientId, ct);

        var now = DateTime.UtcNow;

        bool IsPast(Booking b)
        {
            var end = b.Date.ToDateTime(b.TimeSlot.End);
            return end < now;
        }

        var upcoming = bookings
            .Where(b => !IsPast(b))
            .OrderBy(b => b.Date)
            .ThenBy(b => b.TimeSlot.Start);

        var past = bookings
            .Where(IsPast)
            .OrderByDescending(b => b.Date)
            .ThenByDescending(b => b.TimeSlot.Start);

        return
        [
            .. upcoming
                .Concat(past)
                .Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    Comment = b.Comment,
                    CreatedAt = b.CreatedAt,
                    Date = b.Date,
                    Start = b.TimeSlot.Start,
                    End = b.TimeSlot.End,
                }),
        ];
    }
}
