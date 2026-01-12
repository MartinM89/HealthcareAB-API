using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Repositories.Implementations;

public class BookingRepository(AppDbContext context) : IBookingRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Booking> CreateAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public Task<Booking?> GetByIdWithPatientAsync(Guid bookingId, CancellationToken ct) =>
        _context.Bookings
            .Include(b => b.Patient)
            .FirstOrDefaultAsync(b => b.Id == bookingId, ct);

    public async Task DeleteAsync(Booking booking, CancellationToken ct)
    {
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync(ct);
    }
}
