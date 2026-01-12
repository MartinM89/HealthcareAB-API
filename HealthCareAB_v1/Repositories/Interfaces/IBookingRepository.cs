using HealthCareAB_v1.Models;

namespace HealthCareAB_v1.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<Booking> CreateAsync(Booking booking);
}
