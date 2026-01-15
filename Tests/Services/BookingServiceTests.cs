using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Services;

public class BookingServiceTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly AppDbContext _context;
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_dbContextOptions);
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _bookingService = new BookingService(_bookingRepositoryMock.Object, _context);
    }

    [Fact]
    public async Task CreateAsync_WithExistingPatient_CreatesBookingAndSavesAsync()
    {
        // Arrange (use shared fixtures)
        var patientId = Guid.NewGuid();
        var user = new User
        {
            Id = patientId,
            Username = "test",
            PasswordHash = "h",
            Patient = new Patient { },
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _bookingRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Booking>()))
            .ReturnsAsync((Booking b) => b);

        var dto = new CreateBookingDto
        {
            Comment = "Kommentar",
            Start = new TimeOnly(09, 30),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        var userId = patientId.ToString();
        var before = DateTime.UtcNow;

        // Act
        var result = await _bookingService.CreateAsync(userId, dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(dto.Comment, result.Comment);
        Assert.InRange(result.CreatedAt, before.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
        Assert.Equal(dto.Date, result.Date);
        Assert.Equal(dto.Start, result.TimeSlot.Start);
        Assert.Equal(dto.Start.AddMinutes(30), result.TimeSlot.End);
        Assert.NotNull(result.Patient);

        _bookingRepositoryMock.Verify(
            r =>
                r.CreateAsync(
                    It.Is<Booking>(b => b.Date == dto.Date && b.TimeSlot.Start == dto.Start)
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_WithMissingPatient_ThrowsNotFoundExceptionAndDoesNotCallRepoAsync()
    {
        // Arrange:
        _bookingRepositoryMock.Reset();
        var service = _bookingService;

        var dto = new CreateBookingDto
        {
            Comment = "Kommentar",
            Start = new TimeOnly(10, 0),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        var userId = Guid.NewGuid().ToString();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => service.CreateAsync(userId, dto)
        );
        Assert.Contains("Patient not found", ex.Message);

        _bookingRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Booking>()), Times.Never);
    }
}
