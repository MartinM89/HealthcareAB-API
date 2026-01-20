using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using HealthCareAB_v1.DTOs;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAB_v1.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
public class TimeSlotController(ITimeSlotService timeSlotService) : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService = timeSlotService;

    private string? GetUserId()
    {
        return User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpGet("available-timeslots")]
    public async Task<ActionResult<List<TimeSlotAvailabilityDto>>> GetAvailableTimeSlots(
        [FromQuery] DateOnly date,
        CancellationToken ct
    )
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _timeSlotService.GetAvailableTimeSlotsAsync(date, ct);
        return Ok(result);
    }
}
