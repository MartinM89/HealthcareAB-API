using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using Moq;

namespace Tests.Services;

public class CaregiverServiceTests
{
    private readonly Mock<ICaregiverRepository> _caregiverRepoMock;
    private readonly CaregiverService _caregiverService;

    public CaregiverServiceTests()
    {
        _caregiverRepoMock = new Mock<ICaregiverRepository>();
        _caregiverService = new CaregiverService(_caregiverRepoMock.Object);
    }

    [Fact]
    public async Task GetScheduleOverviewAsync_WhenNoSchedulesExists_ReturnsEmptySchedulesAsync() //Fix suffix name later.
    {
        // Arrange
        var caregiverId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(7);

        _caregiverRepoMock
            .Setup(repo => repo.GetSchedulesWithBookingsAsync(caregiverId, startDate, endDate))
            .ReturnsAsync([]);

        // Act
        var result = await _caregiverService.GetScheduleOverviewAsync(
            caregiverId,
            startDate,
            endDate
        );

        // Assert
        Assert.NotNull(result);
        Assert.Equal(caregiverId, result.CaregiverId);
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);
        Assert.Empty(result.Schedules);
        _caregiverRepoMock.Verify(
            repo => repo.GetSchedulesWithBookingsAsync(caregiverId, startDate, endDate),
            Times.Once
        );
    }
}
