using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking?> GetByIdWithPatientAsync(Guid bookingId, CancellationToken ct);
    Task DeleteAsync(Booking booking, CancellationToken ct);
    Task<List<Booking>> GetForPatientAsync(Guid patientId, CancellationToken ct);
}
