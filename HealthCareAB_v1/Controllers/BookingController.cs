using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using HealthCareAB_v1.DTOs.Booking;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Services.Interfaces;
using HealthCareAB_v1.Services.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAB_v1.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    private string? GetUserId()
    {
        return User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpPost("create-booking")]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _bookingService.CreateAsync(userId, dto);

        return Ok(MapToDto(result));
    }

    [Authorize(Roles = Roles.Patient)]
    [HttpDelete("{bookingId:guid}")]
    public async Task<IActionResult> Cancel(Guid bookingId, CancellationToken ct)
    {
        if (!TryGetUserId(out var patientId))
        {
            return Unauthorized();
        }

        var result = await _bookingService.CancelAsync(bookingId, patientId, ct);

        return result switch
        {
            CancelBookingResult.Cancelled => NoContent(),
            CancelBookingResult.BookingDoesNotExist => NotFound(),
            CancelBookingResult.NotOwnedByPatient => Forbid(),
            CancelBookingResult.Unauthorized => Unauthorized(),
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
    public async Task<ActionResult<List<BookingResponseDto>>> GetByPatientId(CancellationToken ct)
    {
        if (!TryGetUserId(out var patientId))
        {
            return Unauthorized();
        }

        var result = await _bookingService.GetByPatientIdAsync(patientId, ct);
        return Ok(result);
    }

    private static BookingResponseDto MapToDto(Booking booking)
    {
        return new BookingResponseDto
        {
            Id = booking.Id,
            Comment = booking.Comment,
            CreatedAt = booking.CreatedAt,
            Date = booking.Date,
            Start = booking.TimeSlot.Start,
            End = booking.TimeSlot.End,
        };
    }
}
