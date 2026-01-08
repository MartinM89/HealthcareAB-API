using System.Security.Claims;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCareAB_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IAppDbContext _db;

    public BookingController(IAppDbContext db)
    {
        _db = db;
    }

    [Authorize(Roles = Roles.Patient)]
    [HttpDelete("{bookingId:guid}")]
    public async Task<IActionResult> CancelBooking(Guid bookingId, CancellationToken ct)
    {
        if(!TryGetUserId(out var patientId))
            return Unauthorized();

        var booking = await _db.Bookings
            .Include(b => b.Patient)
            .FirstOrDefaultAsync(b => b.Id == bookingId, ct);

        if(booking == null)
            return NotFound();
        
        if(booking.Patient == null || booking.Patient.Id != patientId)
            return Forbid();

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync(ct);

        return NoContent();
    }

    private bool TryGetUserId(out Guid userId)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out userId);
    }
}