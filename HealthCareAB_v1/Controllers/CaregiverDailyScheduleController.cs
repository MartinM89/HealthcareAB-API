using System.Diagnostics.CodeAnalysis;
using HealthCareAB_v1.DTOs.Caregiver;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAB_v1.Controllers;

[ExcludeFromCodeCoverage]
[Authorize(Roles = Roles.Caregiver)]
[ApiController]
[Route("api/[controller]")]
public class CaregiverDailyScheduleController(
    ICaregiverDailyScheduleService caregiverDailyScheduleService
) : ControllerBase
{
    private readonly ICaregiverDailyScheduleService _caregiverDailyScheduleService =
        caregiverDailyScheduleService;

    [HttpPost]
    public async Task<IActionResult> Create(CreateCaregiverDailyScheduleDto dto)
    {
        await _caregiverDailyScheduleService.CreateAsync(dto);

        return Ok();
    }
}
