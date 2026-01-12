using System.Security.Claims;
using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAB_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IAppDbContext _db;
    private readonly IBookingService _bookingService;

    public BookingController(IAppDbContext db, IBookingService bookingService)
    {
        _db = db;
        _bookingService = bookingService;
    }

    private string? GetUserId()
    {
        return User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _bookingService.CreateAsync(userId, dto);

        // tillgänglig vårdgivare behövs
        if (result == null)
        {
            return BadRequest();
        }
        return Ok(result);
    }

    [Authorize(Roles = Roles.Patient)]
    [HttpDelete("{bookingId:guid}")]
    public async Task<IActionResult> CancelBooking(Guid bookingId, CancellationToken ct)
    {
        if (!TryGetUserId(out var patientId))
            return Unauthorized();

        var result = await _bookingService.CancelAsync(bookingId, patientId, ct);

        return result switch
        {
            CancelBookingResult.Success => NoContent(),
            CancelBookingResult.NotFound => NotFound(),
            CancelBookingResult.Forbidden => Forbid(),
            _ => Unauthorized(),
        };
    }

    private bool TryGetUserId(out Guid userId)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out userId);
    }

    [Authorize(Roles = Roles.Patient)]
    [HttpGet("mybookings")]
    public async Task<ActionResult<List<BookingResponseDto>>> GetMyBookings(CancellationToken ct)
    {
        if(!TryGetUserId(out var patientId))
            return Unauthorized();

        var result = await _bookingService.GetMyBookingsAsync(patientId, ct);
        return Ok(result);
    }
}
