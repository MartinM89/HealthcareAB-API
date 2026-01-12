using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;
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
        var patient =
            await _appDbContext.Patients.FirstOrDefaultAsync(p => p.Id == Guid.Parse(userId))
            ?? throw new NotFoundException("Patient not found");

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Comment = dto.Comment ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            Date = dto.Date,
            UserId = userId,
            TimeSlot = new TimeSlot
            {
                Start = dto.Start,
                End = SetEnd(dto.Start, durationInMinutes),
            },
            Patient = patient,
        };

        return await _bookingRepository.CreateAsync(booking);
    }

    public async Task<CancelBookingResult> CancelAsync(Guid bookingId, Guid patentId, CancellationToken ct)
    {
        var booking = await _bookingRepository.GetByIdWithPatientAsync(bookingId, ct);

        if(booking == null)
            return CancelBookingResult.NotFound;

        if(booking.Patient == null || booking.Patient.Id != patentId)
            return CancelBookingResult.Forbidden;

        await _bookingRepository.DeleteAsync(booking, ct);
        return CancelBookingResult.Success;
    }

    private static TimeOnly SetEnd(TimeOnly start, double timeLength)
    {
        var finalTime = timeLength;
        return start.AddMinutes(finalTime);

    }
}
