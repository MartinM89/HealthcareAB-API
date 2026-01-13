using System.Security.Claims;
using HealthCareAB_v1.DTOs.Auth;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAB_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CaregiverController(ICaregiverService caregiverService, IAuthService authService)
    : ControllerBase
{
    private readonly ICaregiverService _caregiverService = caregiverService; //Change to Caregiver service later.
    private readonly IAuthService _authService = authService; //Change to Caregiver service later.

    private string? GetUserId()
    {
        return User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpPost("Create/Caregiver")]
    public async Task<IActionResult> CreateCaregiverAsync(RegisterCaregiverDto registerDto)
    {
        var createdCaregiver = await _authService.RegisterCaregiverAsync(registerDto);

        return Ok(createdCaregiver);
    }

    [HttpGet("GetUpcomingSchedule")]
    [Authorize(Roles = Roles.Caregiver)]
    public async Task<IActionResult> GetUpcomingScheduleAsync()
    {
        var caregiverId = GetUserId();
        if (caregiverId == null)
        {
            return Unauthorized();
        }

        var upcomingScheduleOverview = await _caregiverService.GetUpcomingSchedulesAsync(
            Guid.Parse(caregiverId)
        );

        return Ok(upcomingScheduleOverview);
    }
}
