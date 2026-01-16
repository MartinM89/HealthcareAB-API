using System.Security.Claims;
using HealthCareAB_v1.DTOs.User.Caregiver;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAB_v1.Controllers;

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

    [HttpPost("create-booking")]
    public async Task<IActionResult> CreateBooking([FromBody] CaregiverCreateBookingDto request)
    {
        try
        {
            var caregiverId = GetUserId();

            if (caregiverId == null)
            {
                return Unauthorized(new { message = "You must be logged in as a caregiver" });
            }

            var booking = await _caregiverService.CreateBookingForPatientAsync(
                Guid.Parse(caregiverId),
                request
            );

            return Ok(
                new
                {
                    message = "Booking created successfully",
                    bookingId = booking.Id,
                    patientId = booking.PatientId,
                    date = booking.Date,
                    timeSlotId = booking.TimeSlotId,
                    caregiverDailyScheduleId = booking.CaregiverDailyScheduleId,
                    comment = booking.Comment,
                    createdAt = booking.CreatedAt,
                }
            );
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }

    [HttpGet("get-upcoming-schedule")]
    public async Task<IActionResult> GetUpcomingSchedule()
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
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }
}
