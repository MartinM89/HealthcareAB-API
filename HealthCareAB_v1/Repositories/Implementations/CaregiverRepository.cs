using HealthCareAB_v1.DTOs.User.Caregiver;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Repositories.Implementations;

public class CaregiverRepository(IAppDbContext context) : ICaregiverRepository
{
    private readonly IAppDbContext _context = context;

    public async Task CreateCaregiverAsync(Caregiver caregiver)
    {
        _context.Caregivers.Add(caregiver);
        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<CaregiverDailySchedule>> GetSchedulesWithBookingsAsync(
        Guid caregiverId,
        DateTime startDate,
        DateTime endDate
    )
    {
        var caregiversSchedule = await _context
            .CaregiverDailySchedules.Where(schedule => schedule.CaregiverId == caregiverId)
            .Where(schedule => schedule.StartTime <= endDate && schedule.EndTime >= startDate)
            .Include(schedule => schedule.CaregiverStatus)
            .Include(schedule => schedule.Bookings)
            .ThenInclude(booking => booking.Patient)
            .ThenInclude(patient => patient.User)
            .Include(schedule => schedule.Bookings)
            .ThenInclude(booking => booking.TimeSlot)
            .OrderBy(schedule => schedule.StartTime)
            .ToListAsync();

        return caregiversSchedule;
    }

    public async Task<Patient?> GetPatientByIdAsync(CaregiverCreateBookingDto request)
    {
        return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == request.PatientId);
    }

    public async Task<CaregiverDailySchedule?> GetCaregiversDailyScheduleAsync(
        CaregiverCreateBookingDto request
    )
    {
        return await _context
            .CaregiverDailySchedules.Include(s => s.CaregiverStatus)
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == request.CaregiverDailyScheduleId);
    }

    public async Task<TimeSlot?> GetTimeSlotAsync(CaregiverCreateBookingDto request)
    {
        return await _context.TimeSlots.FirstOrDefaultAsync(ts => ts.Id == request.TimeSlotId);
    }

    public async Task AddBookingAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
    }
}
