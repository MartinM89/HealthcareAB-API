using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Tests.Controllers;

public class BookingServiceAvailableTimeSlotsTests
{
    private static AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static BookingService CreateBookingService(AppDbContext db)
    {
        IBookingRepository bookingRepository = new BookingRepository(db);

        ITimeSlotRepository timeSlotRepository = new TimeSlotRepository(db);
        ITimeSlotService timeSlotService = new TimeSlotService(timeSlotRepository);

        IUserService userService = new UserService(db);

        ICaregiverStatusRepository caregiverStatusRepository = new CaregiverStatusRepository(db);

        ICaregiverDailyScheduleRepository caregiverDailyScheduleRepository =
        new CaregiverDailyScheduleRepository(db);

        ICaregiverDailyScheduleService caregiverDailyScheduleService =
            new CaregiverDailyScheduleService(
                caregiverStatusRepository,
                userService,
                caregiverDailyScheduleRepository
            );

        return new BookingService(
            bookingRepository,
            timeSlotService,
            caregiverDailyScheduleService,
            userService,
            db
        );
    }

    private static CaregiverStatus AvailableStatus() =>
        new() { Id = Guid.NewGuid(), Status = "AVAILABLE" };

    [Fact]
    public async Task GetAvailableTimeSlots_Returns16SlotsAsync()
    {
        using var db = CreateInMemoryDb();
        var service = CreateBookingService(db);

        var date = new DateOnly(2026, 1, 19);

        var status = AvailableStatus();
        var schedule = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            StartTime = date.ToDateTime(new TimeOnly(8, 0)),
            EndTime = date.ToDateTime(new TimeOnly(16, 0)),
            CaregiverStatus = status,
            CaregiverStatusId = status.Id,
            CaregiverId = Guid.NewGuid(),
            Caregiver = new Caregiver { UserId = Guid.NewGuid(), User = new User { Id = Guid.NewGuid(), Username = "c1", PasswordHash = "x" } }
        };

        db.CaregiverStatuses.Add(status);
        db.CaregiverDailySchedules.Add(schedule);
        await db.SaveChangesAsync();

        var result = await service.GetAvailableTimeSlotsAsync(date, CancellationToken.None);

        Assert.Equal(16, result.Count);
        Assert.All(result, r =>
        {
            Assert.False(string.IsNullOrWhiteSpace(r.Id));
            Assert.False(string.IsNullOrWhiteSpace(r.Start));
            Assert.False(string.IsNullOrWhiteSpace(r.End));
        });
    }

    [Fact]
    public async Task GetAvailableTimeSlots_WhenAllSchedulesBookedForSlot_ReturnsNotAvailableAndNullScheduleIdAsync()
    {
        using var db = CreateInMemoryDb();
        var service = CreateBookingService(db);

        var date = new DateOnly(2026, 1, 19);
        var status = AvailableStatus();

        var s1 = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            StartTime = date.ToDateTime(new TimeOnly(8, 0)),
            EndTime = date.ToDateTime(new TimeOnly(16, 0)),
            CaregiverStatus = status,
            CaregiverStatusId = status.Id,
            CaregiverId = Guid.NewGuid(),
            Caregiver = new Caregiver { UserId = Guid.NewGuid(), User = new User { Id = Guid.NewGuid(), Username = "c1", PasswordHash = "x" } }
        };

        var s2 = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            StartTime = date.ToDateTime(new TimeOnly(8, 0)),
            EndTime = date.ToDateTime(new TimeOnly(16, 0)),
            CaregiverStatus = status,
            CaregiverStatusId = status.Id,
            CaregiverId = Guid.NewGuid(),
            Caregiver = new Caregiver { UserId = Guid.NewGuid(), User = new User { Id = Guid.NewGuid(), Username = "c2", PasswordHash = "x" } }
        };

        db.CaregiverStatuses.Add(status);
        db.CaregiverDailySchedules.AddRange(s1, s2);

        var patient = new Patient { UserId = Guid.NewGuid(), User = new User { Id = Guid.NewGuid(), Username = "p1", PasswordHash = "x" } };

        db.Patients.Add(patient);

        db.Bookings.AddRange(
            new Booking
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Date = date,
                Patient = patient,
                TimeSlot = new TimeSlot { Start = new TimeOnly(10, 0), End = new TimeOnly(10, 30) },
                CaregiverDailyScheduleId = s1.Id
            },
            new Booking
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Date = date,
                Patient = patient,
                TimeSlot = new TimeSlot { Start = new TimeOnly(10, 0), End = new TimeOnly(10, 30) },
                CaregiverDailyScheduleId = s2.Id
            }
        );

        await db.SaveChangesAsync();

        var result = await service.GetAvailableTimeSlotsAsync(date, CancellationToken.None);

        var slot = result.Single(r => r.Start == "10:00" && r.End == "10:30");
        Assert.False(slot.IsAvailable);
        Assert.Null(slot.ScheduleId);
    }

    [Fact]
    public async Task GetAvailableTimeSlots_WhenOneScheduleFree_ReturnsAvailableAndThatScheduleIdAsync()
    {
        using var db = CreateInMemoryDb();
        var service = CreateBookingService(db);

        var date = new DateOnly(2026, 1, 19);
        var status = AvailableStatus();

        var s1 = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            StartTime = date.ToDateTime(new TimeOnly(8, 0)),
            EndTime = date.ToDateTime(new TimeOnly(16, 0)),
            CaregiverStatus = status,
            CaregiverStatusId = status.Id,
            CaregiverId = Guid.NewGuid(),
            Caregiver = new Caregiver { UserId = Guid.NewGuid(), User = new User { Id = Guid.NewGuid(), Username = "c1", PasswordHash = "x" } }
        };

        var s2 = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            StartTime = date.ToDateTime(new TimeOnly(8, 0)),
            EndTime = date.ToDateTime(new TimeOnly(16, 0)),
            CaregiverStatus = status,
            CaregiverStatusId = status.Id,
            CaregiverId = Guid.NewGuid(),
            Caregiver = new Caregiver { UserId = Guid.NewGuid(), User = new User { Id = Guid.NewGuid(), Username = "c2", PasswordHash = "x" } }
        };

        db.CaregiverStatuses.Add(status);
        db.CaregiverDailySchedules.AddRange(s1, s2);

        var patient = new Patient { UserId = Guid.NewGuid(), User = new User { Id = Guid.NewGuid(), Username = "p1", PasswordHash = "x" } };
        db.Patients.Add(patient);

        db.Bookings.Add(new Booking
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Date = date,
            Patient = patient,
            TimeSlot = new TimeSlot { Start = new TimeOnly(13, 0), End = new TimeOnly(13, 30) },
            CaregiverDailyScheduleId = s1.Id
        });

        await db.SaveChangesAsync();

        var result = await service.GetAvailableTimeSlotsAsync(date, CancellationToken.None);

        var slot = result.Single(r => r.Start == "13:00" && r.End == "13:30");
        Assert.True(slot.IsAvailable);
        Assert.NotNull(slot.ScheduleId);

        Assert.NotEqual(s1.Id, slot.ScheduleId.Value);
        Assert.True(slot.ScheduleId.Value == s2.Id);
    }
}
