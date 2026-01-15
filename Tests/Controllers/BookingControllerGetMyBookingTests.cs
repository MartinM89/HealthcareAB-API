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

        var patient1Id = Guid.NewGuid();
        var patient2Id = Guid.NewGuid();

        var (user1, patient1) = CreatePatient(patient1Id, "p1");
        var (user2, patient2) = CreatePatient(patient2Id, "p2");

        db.Users.AddRange(user1, user2);
        db.Patients.AddRange(patient1, patient2);

        db.Bookings.Add(new Booking
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Patient = patient1,
            TimeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                Start = new TimeOnly(9, 0),
                End = new TimeOnly(9, 30),
            }
        });

        db.Bookings.Add(new Booking
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Patient = patient2,
            TimeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                Start = new TimeOnly(10, 0),
                End = new TimeOnly(10, 30),
            }
        });

        await db.SaveChangesAsync();

        var controller = CreateController(db, patient1Id);

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
        var (user, patient) = CreatePatient(patientId, "p");
        db.Users.Add(user);
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        var controller = CreateController(db, patientId);

        var action = await controller.GetMyBookingsAsync(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var list = Assert.IsType<List<BookingResponseDto>>(ok.Value);

        Assert.Empty(list);
    }

}