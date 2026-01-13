using HealthCareAB_v1.DTOs.Caregiver;
using HealthCareAB_v1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAB_v1.Controllers;

[Authorize(Roles = Roles.Caregiver)]
[ApiController]
[Route("api/[controller]")]
public class CaregiverController() : ControllerBase
{
    [HttpPost]
    public IActionResult WorkingSchedule(CreateCaregiverDailyScheduleDto dto)
    {
        return Ok(dto);
    }
}
