using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Tests.Controllers;

public class TimeSlotServiceTests
{
    private static AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static TimeSlotService CreateTimeSlotService(AppDbContext db)
    {
        ITimeSlotRepository timeSlotRepository = new TimeSlotRepository(db);

        return new TimeSlotService(timeSlotRepository);
    }

    private static CaregiverStatus AvailableStatus() =>
        new() { Id = Guid.NewGuid(), Status = "AVAILABLE" };

    [Fact]
    public async Task GetAvailableTimeSlots_Returns16SlotsAsync()
    {
        using var db = CreateInMemoryDb();
        var service = CreateTimeSlotService(db);

        var date = new DateOnly(2026, 1, 19);
        var dateTime = new TimeOnly(7, 30);
        var status = AvailableStatus();
        var schedule = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            StartTime = date.ToDateTime(new TimeOnly(8, 0)),
            EndTime = date.ToDateTime(new TimeOnly(16, 0)),
            CaregiverStatus = status,
            CaregiverStatusId = status.Id,
            CaregiverId = Guid.NewGuid(),
            Caregiver = new Caregiver
            {
                UserId = Guid.NewGuid(),
                User = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "c1",
                    PasswordHash = "x",
                },
            },
        };

        for (var i = 0; i < 16; i++)
        {
            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                Start = dateTime,
                End = dateTime.AddMinutes(30),
                Bookings = [],
            };
            timeSlot.Start.AddMinutes(30);
            timeSlot.End.AddMinutes(30);

            db.TimeSlots.Add(timeSlot);
        }

        db.CaregiverStatuses.Add(status);
        db.CaregiverDailySchedules.Add(schedule);
        await db.SaveChangesAsync();

        var result = await service.GetAvailableTimeSlotsAsync(date, CancellationToken.None);

        Assert.Equal(16, result.Count());
        Assert.All(
            result,
            r =>
            {
                Assert.NotEqual(r.Id, Guid.Empty);
                Assert.False(string.IsNullOrWhiteSpace(r.Start));
                Assert.False(string.IsNullOrWhiteSpace(r.End));
            }
        );
    }

    [Fact]
    public async Task GetAvailableTimeSlots_WhenAllSchedulesBookedForSlot_ReturnsNotAvailableAndNullScheduleIdAsync()
    {
        using var db = CreateInMemoryDb();
        var service = CreateTimeSlotService(db);

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
            Caregiver = new Caregiver
            {
                UserId = Guid.NewGuid(),
                User = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "c1",
                    PasswordHash = "x",
                },
            },
        };

        var s2 = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            StartTime = date.ToDateTime(new TimeOnly(8, 0)),
            EndTime = date.ToDateTime(new TimeOnly(16, 0)),
            CaregiverStatus = status,
            CaregiverStatusId = status.Id,
            CaregiverId = Guid.NewGuid(),
            Caregiver = new Caregiver
            {
                UserId = Guid.NewGuid(),
                User = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "c2",
                    PasswordHash = "x",
                },
            },
        };

        var timeOnly = new TimeOnly(7, 30);
        var fixedGuid = Guid.NewGuid();
        for (var i = 0; i < 16; i++)
        {
            timeOnly = timeOnly.AddMinutes(30);

            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                Start = timeOnly,
                End = timeOnly.AddMinutes(30),
                Bookings = [],
            };

            if (i == 0)
            {
                timeSlot.Id = fixedGuid;
            }

            db.TimeSlots.Add(timeSlot);
        }

        db.CaregiverStatuses.Add(status);
        db.CaregiverDailySchedules.AddRange(s1, s2);

        var patient = new Patient
        {
            UserId = Guid.NewGuid(),
            User = new User
            {
                Id = Guid.NewGuid(),
                Username = "p1",
                PasswordHash = "x",
            },
        };

        db.Patients.Add(patient);
        await db.SaveChangesAsync();
        var existingTimeSlot = await service.GetByIdAsync(fixedGuid);

        db.Bookings.AddRange(
            new Booking
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Date = date,
                Patient = patient,
                TimeSlot = existingTimeSlot,
                CaregiverDailyScheduleId = s1.Id,
            },
            new Booking
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Date = date,
                Patient = patient,
                TimeSlot = existingTimeSlot,
                CaregiverDailyScheduleId = s2.Id,
            }
        );

        await db.SaveChangesAsync();

        var result = await service.GetAvailableTimeSlotsAsync(date, CancellationToken.None);

        var slot = result.Single(t => t.Start == "08:00");
        Assert.False(slot.IsAvailable);
        Assert.Null(slot.ScheduleId);
    }

    [Fact]
    public async Task GetAvailableTimeSlots_WhenOneScheduleFree_ReturnsAvailableAndThatScheduleIdAsync()
    {
        using var db = CreateInMemoryDb();
        var service = CreateTimeSlotService(db);

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
            Caregiver = new Caregiver
            {
                UserId = Guid.NewGuid(),
                User = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "c1",
                    PasswordHash = "x",
                },
            },
        };

        var s2 = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            StartTime = date.ToDateTime(new TimeOnly(8, 0)),
            EndTime = date.ToDateTime(new TimeOnly(16, 0)),
            CaregiverStatus = status,
            CaregiverStatusId = status.Id,
            CaregiverId = Guid.NewGuid(),
            Caregiver = new Caregiver
            {
                UserId = Guid.NewGuid(),
                User = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "c2",
                    PasswordHash = "x",
                },
            },
        };

        var timeOnly = new TimeOnly(7, 30);
        var fixedGuid = Guid.NewGuid();
        for (var i = 0; i < 16; i++)
        {
            timeOnly = timeOnly.AddMinutes(30);

            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                Start = timeOnly,
                End = timeOnly.AddMinutes(30),
                Bookings = [],
            };

            if (i == 6)
            {
                timeSlot.Id = fixedGuid;
            }

            db.TimeSlots.Add(timeSlot);
        }

        db.CaregiverStatuses.Add(status);
        db.CaregiverDailySchedules.AddRange(s1, s2);

        var patient = new Patient
        {
            UserId = Guid.NewGuid(),
            User = new User
            {
                Id = Guid.NewGuid(),
                Username = "p1",
                PasswordHash = "x",
            },
        };

        db.Patients.Add(patient);
        await db.SaveChangesAsync();
        var existingTimeSlot = await service.GetByIdAsync(fixedGuid);

        db.Bookings.Add(
            new Booking
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Date = date,
                Patient = patient,
                TimeSlot = existingTimeSlot,
                CaregiverDailyScheduleId = s1.Id,
            }
        );

        await db.SaveChangesAsync();

        var result = await service.GetAvailableTimeSlotsAsync(date, CancellationToken.None);

        var slot = result.Single(r => r.Start == "11:00");
        Assert.True(slot.IsAvailable);
        Assert.NotNull(slot.ScheduleId);

        Assert.NotEqual(s1.Id, slot.ScheduleId.Value);
        Assert.True(slot.ScheduleId.Value == s2.Id);
    }

    [Fact]
    public async Task GetAvailableTimeSlots_WhenNoCaregiverDailySchedulesExist_ReturnEmptyList()
    {
        using var db = CreateInMemoryDb();
        var service = CreateTimeSlotService(db);

        var date = new DateOnly(2026, 1, 19);

        var timeOnly = new TimeOnly(8, 30);
        var fixedGuid = Guid.NewGuid();
        for (var i = 0; i < 16; i++)
        {
            timeOnly = timeOnly.AddMinutes(30);

            var timeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                Start = timeOnly,
                End = timeOnly.AddMinutes(30),
                Bookings = [],
            };

            if (i == 0)
            {
                timeSlot.Id = fixedGuid;
            }

            db.TimeSlots.Add(timeSlot);
        }

        await db.SaveChangesAsync();

        var result = await service.GetAvailableTimeSlotsAsync(date, CancellationToken.None);

        Assert.Empty(result);
    }
}
