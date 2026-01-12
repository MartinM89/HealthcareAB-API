using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Services.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateAsync(string userId, CreateBookingDto dto);
    Task<CancelBookingResult> CancelAsync(Guid bookingId, Guid patientId, CancellationToken ct);

    Task<List<BookingResponseDto>> GetMyBookingsAsync(Guid patientId, CancellationToken ct);
}
