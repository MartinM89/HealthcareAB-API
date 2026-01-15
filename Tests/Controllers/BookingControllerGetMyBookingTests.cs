using System.Security.Claims;
using HealthCareAB_v1.Controllers;
using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Tests.Controllers;

public class BookingControllerGetMyBookingsTests
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

    private static (User user, Patient patient) CreatePatient(Guid userId, string username)
    {
        var user = new User
        {
            Id = userId,
            Username = username,
            PasswordHash = "hash",
            Email = "bo@test.com",
            FirstName = "Bo",
            LastName = "Ek",
            PhoneNumber = "0701234567",
        };

        var patient = new Patient
        {
            UserId = userId,
            User = user,
            SocialSecurityNumber = "196905124816",
            Street = "Bogatan",
            City = "Göteborg",
            ZipCode = "41700",
        };

        user.Patient = patient;

        return (user, patient);
    }

    [Fact]
    public async Task GetMyBookings_NotAuthenticated_ReturnsUnauthorizedAsync()
    {
        using var db = CreateInMemoryDb();
        var controller = CreateController(db, null);

        var action = await controller.GetMyBookingsAsync(CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(action.Result);

    }

    [Fact]
    public async Task GetMyBookings_Authenticated_ReturnsOnlyBookingsForLoggedInPatientAsync()
    {
        using var db = CreateInMemoryDb();

        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();

        var (u1, p1) = CreatePatient(p1Id, "p1");
        var (u2, p2) = CreatePatient(p2Id, "p2");

        db.Users.AddRange(u1, u2);
        db.Patients.AddRange(p1, p2);

        // Booking för patient 1
        db.Bookings.Add(new Booking
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), // framtid
            Patient = p1,
            TimeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                Start = new TimeOnly(9, 0),
                End = new TimeOnly(9, 30),
            }
        });

        // Booking för patient 2
        db.Bookings.Add(new Booking
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), // framtid
            Patient = p2,
            TimeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                Start = new TimeOnly(10, 0),
                End = new TimeOnly(10, 30),
            }
        });

        await db.SaveChangesAsync();

        var controller = CreateController(db, p1Id);

        var action = await controller.GetMyBookingsAsync(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var list = Assert.IsType<List<BookingResponseDto>>(ok.Value);

        Assert.Single(list);
        Assert.Equal(new TimeOnly(9, 0), list[0].Start);
    }

    [Fact]
    public async Task GetMyBookings_NoBookings_ReturnsOkWithEmptyListAsync()
    {
        using var db = CreateInMemoryDb();

        var patientId = Guid.NewGuid();
        var (u, p) = CreatePatient(patientId, "p");
        db.Users.Add(u);
        db.Patients.Add(p);
        await db.SaveChangesAsync();

        var controller = CreateController(db, patientId);

        var action = await controller.GetMyBookingsAsync(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var list = Assert.IsType<List<BookingResponseDto>>(ok.Value);

        Assert.Empty(list);
    }

}