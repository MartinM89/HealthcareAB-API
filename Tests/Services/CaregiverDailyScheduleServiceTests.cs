using HealthCareAB_v1.DTOs.Caregiver;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using HealthCareAB_v1.Services.Interfaces;
using Moq;

namespace Tests.Services;

public class CaregiverDailyScheduleServiceTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ICaregiverStatusRepository> _statusRepoMock;
    private readonly Mock<ICaregiverDailyScheduleRepository> _repoMock;
    private readonly CaregiverDailyScheduleService _service;

    public CaregiverDailyScheduleServiceTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _statusRepoMock = new Mock<ICaregiverStatusRepository>();
        _repoMock = new Mock<ICaregiverDailyScheduleRepository>();

        _service = new CaregiverDailyScheduleService(
            _statusRepoMock.Object,
            _userServiceMock.Object,
            _repoMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedSchedule_WhenValidAsync()
    {
        // Arrange
        var caregiverId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var dto = new CreateCaregiverDailyScheduleDto(date, caregiverId, statusId);

        var caregiver = new Caregiver
        {
            UserId = caregiverId,
            User = new User
            {
                Id = Guid.NewGuid(),
                Username = "u",
                PasswordHash = "h",
            },
        };
        var status = new CaregiverStatus { Id = statusId, Status = "AVAILABLE" };

        _userServiceMock.Setup(u => u.GetCaregiverByIdAsync(caregiverId)).ReturnsAsync(caregiver);

        _statusRepoMock.Setup(s => s.GetByIdAsync(statusId)).ReturnsAsync(status);

        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<CaregiverDailySchedule>()))
            .ReturnsAsync((CaregiverDailySchedule s) => s);

        var expectedStart = date.ToDateTime(new TimeOnly(8, 0)).ToUniversalTime();
        var expectedEnd = date.ToDateTime(new TimeOnly(16, 0)).ToUniversalTime();

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(caregiverId, result.CaregiverId);
        Assert.Equal(statusId, result.CaregiverStatusId);
        Assert.Equal(expectedStart, result.StartTime);
        Assert.Equal(expectedEnd, result.EndTime);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<CaregiverDailySchedule>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenDateTooFarAsync()
    {
        // Arrange
        var dto = new CreateCaregiverDailyScheduleDto(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(31)),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsNotFoundException_WhenCaregiverNotFoundAsync()
    {
        // Arrange
        var caregiverId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var dto = new CreateCaregiverDailyScheduleDto(date, caregiverId, statusId);

        _userServiceMock
            .Setup(u => u.GetCaregiverByIdAsync(caregiverId))
            .ReturnsAsync((Caregiver?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsNotFoundException_WhenStatusNotFoundAsync()
    {
        // Arrange
        var caregiverId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var dto = new CreateCaregiverDailyScheduleDto(date, caregiverId, statusId);

        var caregiver = new Caregiver
        {
            UserId = caregiverId,
            User = new User
            {
                Id = Guid.NewGuid(),
                Username = "u",
                PasswordHash = "h",
            },
        };

        _userServiceMock.Setup(u => u.GetCaregiverByIdAsync(caregiverId)).ReturnsAsync(caregiver);

        _statusRepoMock.Setup(s => s.GetByIdAsync(statusId)).ReturnsAsync((CaregiverStatus?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsSchedule_WhenFoundAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var schedule = new CaregiverDailySchedule
        {
            Id = id,
            StartTime = date.ToDateTime(new TimeOnly(8, 0)).ToUniversalTime(),
            EndTime = date.ToDateTime(new TimeOnly(16, 0)).ToUniversalTime(),
        };

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(schedule);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Equal(id, result.Id);
        Assert.Equal(schedule.StartTime, result.StartTime);
        Assert.Equal(schedule.EndTime, result.EndTime);
        _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsValidationException_WhenGuidEmptyAsync()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.GetByIdAsync(Guid.Empty));
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenNotFoundAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CaregiverDailySchedule?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(id));
    }
}
