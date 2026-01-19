using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Services;

public class BookingServiceTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly BookingService _bookingService;
    private readonly Mock<ITimeSlotService> _timeSlotServiceMock;
    private readonly Mock<ICaregiverDailyScheduleService> _caregiverDailyScheduleServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly AppDbContext _context;

    public BookingServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_dbContextOptions);
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _timeSlotServiceMock = new Mock<ITimeSlotService>();
        _caregiverDailyScheduleServiceMock = new Mock<ICaregiverDailyScheduleService>();
        _userServiceMock = new Mock<IUserService>();

        _bookingService = new BookingService(
            _bookingRepositoryMock.Object,
            _timeSlotServiceMock.Object,
            _caregiverDailyScheduleServiceMock.Object,
            _userServiceMock.Object
        );
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

        // Prepare ids
        var timeslotId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();

        // Ensure patient's UserId matches
        user.Patient = new Patient { UserId = user.Id, User = user };

        // Mock user service to return the patient
        _userServiceMock.Setup(s => s.GetPatientByIdAsync(user.Id)).ReturnsAsync(user.Patient);

        // Setup a timeslot that matches the dto start
        var expectedEnd = new TimeOnly(10, 0);
        var timeslot = new TimeSlot
        {
            Id = timeslotId,
            Start = new TimeOnly(9, 30),
            End = expectedEnd,
        };
        _timeSlotServiceMock.Setup(s => s.GetByIdAsync(timeslotId)).ReturnsAsync(timeslot);

        // Setup a dummy schedule
        var schedule = new CaregiverDailySchedule { Id = scheduleId };
        _caregiverDailyScheduleServiceMock
            .Setup(s => s.GetByIdAsync(scheduleId))
            .ReturnsAsync(schedule);
        var dto = new CreateBookingDto
        {
            Comment = "Kommentar",
            Start = new TimeOnly(09, 30),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            TimeSlotId = timeslotId,
            ScheduleId = scheduleId,
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
        Assert.Equal(expectedEnd, result.TimeSlot.End);
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
