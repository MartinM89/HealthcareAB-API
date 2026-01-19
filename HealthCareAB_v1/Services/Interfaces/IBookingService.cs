using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Services.Results;

namespace HealthCareAB_v1.Services.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateAsync(string userId, CreateBookingDto dto);
    Task<CancelBookingResult> CancelAsync(Guid bookingId, Guid patientId, CancellationToken ct);

    Task<List<BookingResponseDto>> GetByPatientIdAsync(Guid patientId, CancellationToken ct);
}
