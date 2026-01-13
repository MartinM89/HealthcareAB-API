using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Services;

public class TimeSlotServiceTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
    private readonly AppDbContext _context;
    private readonly Mock<ITimeSlotRepository> _timeSlotRepositoryMock;
    private readonly TimeSlotService _timeSlotService;

    public TimeSlotServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_dbContextOptions);
        _timeSlotRepositoryMock = new Mock<ITimeSlotRepository>();
        _timeSlotService = new TimeSlotService(_timeSlotRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_TimeSlotNotNull_ReturnTimeSlot()
    {
        // Arrange
        var timeSlotId = Guid.NewGuid();
        var currentTime = new TimeOnly();

        var newTimeSlot = new TimeSlot
        {
            Id = timeSlotId,
            Start = currentTime,
            End = currentTime.AddMinutes(30),
            Bookings = [],
        };

        _context.TimeSlots.Add(newTimeSlot);
        await _context.SaveChangesAsync();

        _timeSlotRepositoryMock.Setup(r => r.GetByIdAsync(timeSlotId)).ReturnsAsync(newTimeSlot);

        // Act
        var result = await _timeSlotService.GetByIdAsync(timeSlotId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(timeSlotId, result.Id);
        Assert.Equal(newTimeSlot.Start, result.Start);
        Assert.Equal(newTimeSlot.End, result.End);
        Assert.Equal(newTimeSlot.Bookings, result.Bookings);
        Assert.Empty(result.Bookings);

        _timeSlotRepositoryMock.Verify(x => x.GetByIdAsync(timeSlotId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_TimeSlotIsNull_ThrowNotFoundException()
    {
        // Arrange
        var timeSlotId = Guid.NewGuid();

        // Act/Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _timeSlotService.GetByIdAsync(timeSlotId)
        );
        Assert.Equal("Time slot not found", exception.Message);

        _timeSlotRepositoryMock.Verify(x => x.GetByIdAsync(timeSlotId), Times.Once);
    }
}
