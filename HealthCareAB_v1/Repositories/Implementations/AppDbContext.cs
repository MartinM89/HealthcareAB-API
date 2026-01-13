using System.Text.Json;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HealthCareAB_v1.Repositories.Implementations;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options),
        IAppDbContext
{
    private readonly JsonSerializerOptions _jsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseSerialColumns();

        var rolesConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, _jsonOptions),
            v => JsonSerializer.Deserialize<List<string>>(v, _jsonOptions) ?? new List<string>()
        );

        modelBuilder
            .Entity<User>()
            .Property(e => e.Roles)
            .HasConversion(rolesConverter)
            .HasColumnType("jsonb");

        modelBuilder
            .Entity<Patient>()
            .HasOne(p => p.Review)
            .WithOne(r => r.Patient)
            .HasForeignKey<Review>(r => r.Id);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<Caregiver> Caregivers { get; set; }
    public DbSet<CaregiverDailySchedule> CaregiverDailySchedules { get; set; }
    public DbSet<CaregiverStatus> CaregiverStatuses { get; set; }

    // public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    // {
    //     return base.SaveChangesAsync(cancellationToken);
    // }
}
