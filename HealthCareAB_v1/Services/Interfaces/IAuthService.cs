using HealthCareAB_v1.DTOs.Auth;

namespace HealthCareAB_v1.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterPatientAsync(RegisterPatientDto registerDto);
    Task<AuthResponseDto> RegisterCaregiverAsync(RegisterCaregiverDto registerDto);
    Task<(AuthResponseDto response, string? token)> LoginAsync(LoginDto loginDto);
    CookieOptions GetJwtCookieOptions();
    CookieOptions GetClearCookieOptions();
}
