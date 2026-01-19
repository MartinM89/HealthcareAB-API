using HealthCareAB_v1.DTOs;
using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;
using HealthCareAB_v1.Services.Results;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Services.Implementations;

public class BookingService(IBookingRepository bookingRepository, AppDbContext appDbContext)
    : IBookingService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private const double durationInMinutes = 30;

    public async Task<Booking> CreateAsync(string userId, CreateBookingDto dto)
    {
        var user =
            await _appDbContext
                .Users.Include(u => u.Patient)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId))
            ?? throw new NotFoundException("Patient not found");

        if (user.Patient is null)
        {
            throw new NotFoundException("Patient not found");
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Comment = dto.Comment ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            Date = dto.Date,
            TimeSlot = new TimeSlot
            {
                Start = dto.Start,
                End = SetEnd(dto.Start, durationInMinutes),
            },
            Patient = user.Patient,
            CaregiverDailyScheduleId = dto.CaregiverDailyScheduleId
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

    private static TimeOnly SetEnd(TimeOnly start, double timeLength)
    {
        var finalTime = timeLength;
        return start.AddMinutes(finalTime);
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

    public async Task<List<TimeSlotAvailabilityDto>> GetAvailableTimeSlotsAsync(DateOnly selectedDate, CancellationToken ct)
    {
        var slots = new (TimeOnly Start, TimeOnly End)[]
        {
        (new TimeOnly(8,0),  new TimeOnly(8,30)),
        (new TimeOnly(8,30), new TimeOnly(9,0)),
        (new TimeOnly(9,0),  new TimeOnly(9,30)),
        (new TimeOnly(9,30), new TimeOnly(10,0)),
        (new TimeOnly(10,0), new TimeOnly(10,30)),
        (new TimeOnly(10,30),new TimeOnly(11,0)),
        (new TimeOnly(11,0), new TimeOnly(11,30)),
        (new TimeOnly(11,30),new TimeOnly(12,0)),
        (new TimeOnly(12,0), new TimeOnly(12,30)),
        (new TimeOnly(12,30),new TimeOnly(13,0)),
        (new TimeOnly(13,0), new TimeOnly(13,30)),
        (new TimeOnly(13,30),new TimeOnly(14,0)),
        (new TimeOnly(14,0), new TimeOnly(14,30)),
        (new TimeOnly(14,30),new TimeOnly(15,0)),
        (new TimeOnly(15,0), new TimeOnly(15,30)),
        (new TimeOnly(15,30),new TimeOnly(16,0)),
        };

        var bookings = await _appDbContext.Bookings
    .AsNoTracking()
    .Where(b => b.Date == selectedDate)
    .Select(b => new
    {
        b.TimeSlot.Start,
        b.TimeSlot.End,
        b.CaregiverDailyScheduleId
    })
    .ToListAsync(ct);

        var dayStart = selectedDate.ToDateTime(TimeOnly.MinValue);
        var dayEnd = dayStart.AddDays(1);

        var schedules = await _appDbContext.CaregiverDailySchedules
            .AsNoTracking()
            .Include(s => s.CaregiverStatus)
            .Where(s => s.StartTime >= dayStart && s.StartTime < dayEnd)
            .Where(s => s.CaregiverStatus.Status == "AVAILABLE")
            .ToListAsync(ct);

        var result = new List<TimeSlotAvailabilityDto>(slots.Length);

        foreach (var (Start, End) in slots)
        {
            var slotStartDt = selectedDate.ToDateTime(Start);
            var slotEndDt = selectedDate.ToDateTime(End);

            var candidates = schedules
                .Where(s => s.StartTime <= slotStartDt && s.EndTime >= slotEndDt)
                .Select(s => s.Id)
                .ToList();

            var bookedScheduleIds = bookings
                .Where(b => b.Start == Start && b.End == End)
                .Select(b => b.CaregiverDailyScheduleId)
                .ToHashSet();

            var freeScheduleId = candidates.FirstOrDefault(id => !bookedScheduleIds.Contains(id));

            var startStr = $"{Start:HH\\:mm}";
            var endStr = $"{End:HH\\:mm}";

            var isAvailable = freeScheduleId != Guid.Empty;

            result.Add(new TimeSlotAvailabilityDto
            {
                Id = $"{startStr}-{endStr}",
                Start = startStr,
                End = endStr,
                IsAvailable = isAvailable,
                ScheduleId = isAvailable ? freeScheduleId : null
            });
        }

        return result;
    }
}
