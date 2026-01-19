using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken ct);
    Task DeleteAsync(Booking booking, CancellationToken ct);

    Task<List<Booking>> GetByPatientIdAsync(Guid patientId, CancellationToken ct);
}
