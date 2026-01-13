using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using Moq;

namespace Tests.Services;

public class TimeSlotServiceTests
{
    private readonly Mock<ITimeSlotRepository> _timeSlotRepositoryMock;
    private readonly TimeSlotService _timeSlotService;

    public TimeSlotServiceTests()
    {
        _timeSlotRepositoryMock = new Mock<ITimeSlotRepository>();
        _timeSlotService = new TimeSlotService(_timeSlotRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnTimeSlot()
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

        _timeSlotRepositoryMock.Verify(r => r.GetByIdAsync(timeSlotId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ThrowNotFoundException()
    {
        // Arrange
        var timeSlotId = Guid.NewGuid();

        // Act
        _timeSlotRepositoryMock
            .Setup(r => r.GetByIdAsync(timeSlotId))
            .ReturnsAsync((TimeSlot?)null);

        // Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _timeSlotService.GetByIdAsync(timeSlotId)
        );

        Assert.Equal("Time slot not found", exception.Message);

        _timeSlotRepositoryMock.Verify(r => r.GetByIdAsync(timeSlotId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithEmptyGuid_ThrowValidationException()
    {
        // Arrange
        var timeSlotId = Guid.Empty;

        // Act
        _timeSlotRepositoryMock
            .Setup(r => r.GetByIdAsync(timeSlotId))
            .ReturnsAsync((TimeSlot?)null);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _timeSlotService.GetByIdAsync(timeSlotId)
        );

        Assert.Equal("Guid can't be empty", exception.Message);

        _timeSlotRepositoryMock.Verify(r => r.GetByIdAsync(timeSlotId), Times.Never);
    }
}
