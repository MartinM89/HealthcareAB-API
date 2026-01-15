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
        };

        return await _bookingRepository.CreateAsync(booking);
    }

    public async Task<CancelBookingResult> CancelAsync(
        Guid bookingId,
        Guid patientId,
        CancellationToken ct
    )
    {
        var booking = await _bookingRepository.GetByIdWithPatientAsync(bookingId, ct);

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

    public async Task<List<BookingResponseDto>> GetMyBookingsAsync(Guid patientId, CancellationToken ct)
    {
        var bookings = await _bookingRepository.GetForPatientAsync(patientId, ct);

        return [.. bookings.Select(b => new BookingResponseDto
        {
            Id = b.Id,
            Comment = b.Comment,
            CreatedAt = b.CreatedAt,
            Date = b.Date,
            Start = b.TimeSlot.Start,
            End = b.TimeSlot.End,
        })];
    }
}
