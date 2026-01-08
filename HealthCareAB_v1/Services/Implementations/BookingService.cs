using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;

namespace HealthCareAB_v1.Services.Implementations;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;

    public async Task<Booking> CreateAsync(string userId, CreateBookingDto dto)
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Comment = dto.Comment ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UserId = userId,
            TimeSlot = new TimeSlot { Start = dto.Start },
        };

        return await _bookingRepository.CreateAsync(booking);
    }
}
