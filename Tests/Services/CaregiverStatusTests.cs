using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using Moq;

namespace Tests.Services;

public class CaregiverStatusServiceTests
{
    private readonly Mock<ICaregiverStatusRepository> _statusRepoMock;
    private readonly CaregiverStatusService _service;

    public CaregiverStatusServiceTests()
    {
        _statusRepoMock = new Mock<ICaregiverStatusRepository>();
        _service = new CaregiverStatusService(_statusRepoMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsStatus_WhenFound()
    {
        var id = Guid.NewGuid();
        var status = new CaregiverStatus { Id = id, Status = "AVAILABLE" };

        _statusRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(status);

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("AVAILABLE", result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsValidation_ForEmptyGuid()
    {
        await Assert.ThrowsAsync<ValidationException>(() => _service.GetByIdAsync(Guid.Empty));
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _statusRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CaregiverStatus?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(id));
    }
}
