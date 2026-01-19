using HealthCareAB_v1.Models;
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
    public async Task GetScheduleOverviewAsync_WhenNoSchedulesExists_ReturnsEmptySchedules()
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

    [Fact]
    public async Task GetScheduleOverviewAsync_WhenScheduleHasNoBookings_ReturnsScheduleWithNoBookings()
    {
        // Arrange
        var caregiverId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(7);

        var scheduleDate = startDate.AddDays(1);
        var scheduleStart = scheduleDate.AddHours(8);
        var scheduleEnd = scheduleDate.AddHours(16);

        var schedule = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            CaregiverId = caregiverId,
            CaregiverStatusId = statusId,
            StartTime = scheduleStart,
            EndTime = scheduleEnd,
            CaregiverStatus = new CaregiverStatus { Id = statusId, Status = "AVAILABLE" },
            Bookings = [],
        };

        _caregiverRepoMock
            .Setup(r => r.GetSchedulesWithBookingsAsync(caregiverId, startDate, endDate))
            .ReturnsAsync([schedule]);

        // Act
        var result = await _caregiverService.GetScheduleOverviewAsync(
            caregiverId,
            startDate,
            endDate
        );

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Schedules);

        var resultSchedule = result.Schedules.First();
        Assert.Equal(schedule.Id, resultSchedule.Id);
        Assert.Equal(scheduleStart, resultSchedule.Start);
        Assert.Equal(scheduleEnd, resultSchedule.End);
        Assert.Equal(DateOnly.FromDateTime(scheduleStart), resultSchedule.Date);
        Assert.Equal("AVAILABLE", resultSchedule.Status);
        Assert.Empty(resultSchedule.Bookings);
    }

    [Fact]
    public async Task GetScheduleOverviewAsync_WhenScheduleHasMultipleBookings_ReturnsScheduleWithBookings()
    {
        // Arrange
        var caregiverId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(7);

        var scheduleDate = startDate.AddDays(1);
        var scheduleStart = scheduleDate.AddHours(8);
        var scheduleEnd = scheduleDate.AddHours(16);

        var patientOneId = Guid.NewGuid();
        var patientOne = new Patient
        {
            UserId = patientOneId,
            User = new User
            {
                Id = patientOneId,
                Username = "patient1",
                FirstName = "Martin",
                LastName = "Johansson",
                PhoneNumber = "0763101010",
                PasswordHash = "brakille",
            },
        };

        var patientTwoId = Guid.NewGuid();
        var patientTwo = new Patient
        {
            UserId = patientTwoId,
            User = new User
            {
                Id = patientTwoId,
                Username = "patient2",
                FirstName = "Jimmy",
                LastName = "Michail",
                PhoneNumber = "0710101063",
                PasswordHash = "trotsallt",
            },
        };

        var booking1 = new Booking
        {
            Id = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(scheduleDate),
            Comment = "Kontroll min rygg",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Patient = patientOne,
            TimeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                Start = new TimeOnly(9, 0),
                End = new TimeOnly(9, 30),
            },
        };

        var booking2 = new Booking
        {
            Id = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(scheduleDate),
            Comment = "Kontroll mina axlar",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Patient = patientTwo,
            TimeSlot = new TimeSlot
            {
                Id = Guid.NewGuid(),
                Start = new TimeOnly(14, 0),
                End = new TimeOnly(14, 30),
            },
        };

        var schedule = new CaregiverDailySchedule
        {
            Id = Guid.NewGuid(),
            CaregiverId = caregiverId,
            CaregiverStatusId = statusId,
            StartTime = scheduleStart,
            EndTime = scheduleEnd,
            CaregiverStatus = new CaregiverStatus { Id = statusId, Status = "AVAILABLE" },
            Bookings = [booking2, booking1],
        };

        _caregiverRepoMock
            .Setup(r => r.GetSchedulesWithBookingsAsync(caregiverId, startDate, endDate))
            .ReturnsAsync([schedule]);

        // Act
        var result = await _caregiverService.GetScheduleOverviewAsync(
            caregiverId,
            startDate,
            endDate
        );

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Schedules);

        var resultSchedule = result.Schedules.First();
        Assert.Equal(2, resultSchedule.Bookings.Count);

        var bookingsList = resultSchedule.Bookings.ToList();
        Assert.Equal(booking1.Id, bookingsList[0].Id);
        Assert.Equal(booking2.Id, bookingsList[1].Id);

        Assert.Equal("Kontroll min rygg", bookingsList[0].Comment);
        Assert.Equal("Martin", bookingsList[0].Patient.FirstName);
        Assert.Equal("Johansson", bookingsList[0].Patient.LastName);
        Assert.Equal(new TimeOnly(9, 0), bookingsList[0].TimeSlot.Start);

        Assert.Equal("Kontroll mina axlar", bookingsList[1].Comment);
        Assert.Equal("Jimmy", bookingsList[1].Patient.FirstName);
        Assert.Equal("Michail", bookingsList[1].Patient.LastName);
        Assert.Equal(new TimeOnly(14, 0), bookingsList[1].TimeSlot.Start);
    }

    [Fact]
    public async Task GetScheduleOverviewAsync_WhenCaregiverIdIsEmpty_ThrowArgumentException()
    {
        // Arrange
        var caregiverId = Guid.Empty;
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(7);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _caregiverService.GetScheduleOverviewAsync(caregiverId, startDate, endDate)
        );

        Assert.Equal("Caregiver ID cannot be empty", exception.Message);

        _caregiverRepoMock.Verify(
            repo =>
                repo.GetSchedulesWithBookingsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task GetScheduleOverviewAsync_WhenEndDateIsBeforeStartDate_ThrowArgumentException()
    {
        // Arrange
        var caregiverId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.Date.AddDays(-7);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _caregiverService.GetScheduleOverviewAsync(caregiverId, startDate, endDate)
        );

        Assert.Equal("End date cannot be before start date", exception.Message);

        _caregiverRepoMock.Verify(
            repo =>
                repo.GetSchedulesWithBookingsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task GetScheduleOverviewAsync_WhenDateRangeExceedsThirtyDays_ThrowArgumentException()
    {
        // Arrange
        var caregiverId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.Date.AddDays(31);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _caregiverService.GetScheduleOverviewAsync(caregiverId, startDate, endDate)
        );

        Assert.Equal("Date range cannot exceed 30 days", exception.Message);

        _caregiverRepoMock.Verify(
            repo =>
                repo.GetSchedulesWithBookingsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()
                ),
            Times.Never
        );
    }
}
