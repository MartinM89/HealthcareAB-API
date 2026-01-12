using System.Security.Claims;
using HealthCareAB_v1.Controllers;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Services.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class BookingControllerCancelBookingTests
{
    private static AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new AppDbContext(options);
    }

    private static BookingController CreateController(AppDbContext db, Guid? userId = null)
    {
        IBookingRepository bookingRepository = new BookingRepository(db);
        IBookingService bookingService = new BookingService(bookingRepository, db);

        var controller = new BookingController(db, bookingService);

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

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        return controller;
    }

    [Fact]
    public async Task CancelBooking_OwnBooking_ReturnsNoContent_AndDeletesBooking()
    {
        using var db = CreateInMemoryDb();

        var patientId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();

        var patient = new Patient
        {
            Id = patientId,
            Username = "p1",
            PasswordHash = "hash",
        };

        var booking = new Booking
        {
            Id = bookingId,
            CreatedAt = DateTime.UtcNow,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            UserId = patientId.ToString(),
            Patient = patient,
            TimeSlot = new TimeSlot { Start = new TimeOnly(10, 0) }
        };

        db.Patients.Add(patient);
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        var controller = CreateController(db, patientId);

        var result = await controller.CancelBooking(bookingId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.False(await db.Bookings.AnyAsync(b => b.Id == bookingId));
    }

    [Fact]
    public async Task CancelBooking_NotOwner_ReturnsForbid_AndDoesNotDelete()
    {
        using var db = CreateInMemoryDb();

        var ownerId = Guid.NewGuid();
        var otherPatientsId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();

        var owner = new Patient
        {
            Id = ownerId,
            Username = "owner",
            PasswordHash = "hash",
        };

        var booking = new Booking
        {
            Id = bookingId,
            CreatedAt = DateTime.UtcNow,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            UserId = ownerId.ToString(),
            Patient = owner,
            TimeSlot = new TimeSlot { Start = new TimeOnly(10, 0) }
        };

        db.Patients.Add(owner);
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        var controller = CreateController(db, otherPatientsId);

        var result = await controller.CancelBooking(bookingId, CancellationToken.None);

        Assert.IsType<ForbidResult>(result);
        Assert.True(await db.Bookings.AnyAsync(b => b.Id == bookingId));
    }

    [Fact]
    public async Task CancelBooking_BookingNotFound_ReturnsNotFound()
    {
        using var db = CreateInMemoryDb();

        var patientId = Guid.NewGuid();
        var missingBookingId = Guid.NewGuid();

        var controller = CreateController(db, patientId);

        var result = await controller.CancelBooking(missingBookingId, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }
}