using HealthCareAB_v1.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Repositories.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Patient> Patients { get; set; }
    DbSet<Booking> Bookings { get; set; }
    DbSet<TimeSlot> TimeSlots { get; set; }
    DbSet<Caregiver> Caregivers { get; set; }
    DbSet<CaregiverDailySchedule> CaregiverDailySchedules { get; set; }
    DbSet<CaregiverStatus> CaregiverStatuses { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
