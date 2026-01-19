using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAB_v1.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Caregiver)]
public class CaregiverController(ICaregiverService caregiverService) : ControllerBase
{
    private readonly ICaregiverService _caregiverService = caregiverService;

    private string? GetUserId()
    {
        return User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpGet("GetUpcomingSchedule")]
    public async Task<IActionResult> GetUpcomingScheduleAsync()
    {
        try
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
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(400, new { message = ex.Message });
        }
    }
}
