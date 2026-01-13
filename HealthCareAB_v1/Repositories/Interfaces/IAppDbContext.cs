using HealthCareAB_v1.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Repositories.Interfaces
{
    public interface IAppDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Caregiver> Caregivers { get; set; }
        public DbSet<CaregiverDailySchedule> CaregiverDailySchedules { get; set; }
        public DbSet<CaregiverStatus> CaregiverStatuses { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
