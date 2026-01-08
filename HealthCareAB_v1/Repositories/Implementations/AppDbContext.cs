using System.Text.Json;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HealthCareAB_v1.Repositories.Implementations
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : DbContext(options),
            IAppDbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseSerialColumns();

            var rolesConverter = new ValueConverter<List<string>, string>(
                v =>
                    JsonSerializer.Serialize(
                        v,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ),
                v =>
                    JsonSerializer.Deserialize<List<string>>(
                        v,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ) ?? new List<string>()
            );

            modelBuilder
                .Entity<Patient>()
                .Property(e => e.Roles)
                .HasConversion(rolesConverter)
                .HasColumnType("jsonb");

            modelBuilder
                .Entity<Patient>()
                .HasOne(p => p.Review)
                .WithOne(r => r.Patient)
                .HasForeignKey<Review>(r => r.Id);
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Caregiver> Caregivers { get; set; }
        public DbSet<CaregiverDailySchedule> GetCaregiverDailySchedules { get; set; }
        public DbSet<CaregiverStatus> GetCaregiverStatuses { get; set; }

        // public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        // {
        //     return base.SaveChangesAsync(cancellationToken);
        // }
    }
}
