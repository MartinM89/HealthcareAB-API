using System.Security.Claims;
using HealthCareAB_v1.Controllers;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Tests.Controllers;

public class BookingControllerCancelBookingTests
{
    private static AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static BookingController CreateBookingController(AppDbContext db, Guid? userId = null)
    {
        IBookingRepository bookingRepository = new BookingRepository(db);

        var timeSlotRepository = new TimeSlotRepository(db);

        ITimeSlotService timeSlotService = new TimeSlotService(timeSlotRepository);

        IUserService userService = new UserService(db);

        var caregiverStatusRepository = new CaregiverStatusRepository(db);

        var caregiverDailyScheduleRepository = new CaregiverDailyScheduleRepository(db);

        ICaregiverDailyScheduleService caregiverDailyScheduleService =
            new CaregiverDailyScheduleService(
                caregiverStatusRepository,
                userService,
                caregiverDailyScheduleRepository
            );

        IBookingService bookingService = new BookingService(
            bookingRepository,
            timeSlotService,
            caregiverDailyScheduleService,
            userService,
            db
        );

        var controller = new BookingController(bookingService);

        var httpContext = new DefaultHttpContext();

        if (userId != null)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
                new Claim(ClaimTypes.Role, Roles.Patient),
            };

            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        return controller;
    }

    [Fact]
    public async Task CancelBooking_OwnBooking_ReturnsNoContent_AndDeletesBookingAsync()
    {
        using var db = CreateInMemoryDb();

        var userId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "p1",
            PasswordHash = "hash",
            Patient = new Patient { },
        };

        var booking = new Booking
        {
            Id = bookingId,
            CreatedAt = DateTime.UtcNow,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Patient = user.Patient,
            TimeSlot = new TimeSlot { Start = new TimeOnly(10, 0) },
        };

        db.Users.Add(user);
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        var controller = CreateBookingController(db, userId);

        var result = await controller.Cancel(bookingId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.False(await db.Bookings.AnyAsync(b => b.Id == bookingId));
    }

    [Fact]
    public async Task CancelBooking_NotOwner_ReturnsForbidden_AndDoesNotDeleteAsync()
    {
        using var db = CreateInMemoryDb();

        var ownerId = Guid.NewGuid();
        var otherPatientsId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();

        var owner = new User
        {
            Id = ownerId,
            Username = "owner",
            PasswordHash = "hash",
            Patient = new Patient { },
        };

        var booking = new Booking
        {
            Id = bookingId,
            CreatedAt = DateTime.UtcNow,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Patient = owner.Patient,
            TimeSlot = new TimeSlot { Start = new TimeOnly(10, 0) },
        };

        db.Users.Add(owner);
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        var controller = CreateBookingController(db, otherPatientsId);

        var result = await controller.Cancel(bookingId, CancellationToken.None);

        Assert.IsType<ForbidResult>(result);
        Assert.True(await db.Bookings.AnyAsync(b => b.Id == bookingId));
    }

    [Fact]
    public async Task CancelBooking_BookingNotFound_ReturnsNotFoundAsync()
    {
        using var db = CreateInMemoryDb();

        var patientId = Guid.NewGuid();
        var missingBookingId = Guid.NewGuid();

        var controller = CreateBookingController(db, patientId);

        var result = await controller.Cancel(missingBookingId, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }
}
