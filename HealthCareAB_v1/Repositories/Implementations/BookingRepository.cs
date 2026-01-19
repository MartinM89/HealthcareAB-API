using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Repositories.Implementations;

public class BookingRepository(AppDbContext context) : IBookingRepository
{
    private readonly AppDbContext _context = context;

    // public async Task<CaregiverDailySchedule> GetCaregiverScheduleAsync() { }

    public async Task<Caregiver?> GetCaregiverByIdAsync(Guid caregiverId)
    {
        var caregiver = await _context.Caregivers.FirstOrDefaultAsync(c =>
            c.User.Id == caregiverId
        );

        return caregiver;
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public Task<Booking?> GetByIdAsync(Guid bookingId, CancellationToken ct) =>
        _context
            .Bookings.Include(b => b.Patient)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(b => b.Id == bookingId, ct);

    public async Task DeleteAsync(Booking booking, CancellationToken ct)
    {
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<Booking>> GetForPatientAsync(Guid patientId, CancellationToken ct) =>
        await _context
            .Bookings.AsNoTracking()
            .Include(b => b.TimeSlot)
            .Where(b => b.Patient.UserId == patientId)
            .OrderBy(b => b.Date)
            .ThenBy(b => b.TimeSlot.Start)
            .ToListAsync(ct);
}
