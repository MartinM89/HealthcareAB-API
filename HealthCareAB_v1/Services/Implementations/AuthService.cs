using HealthCareAB_v1.Configuration;
using HealthCareAB_v1.DTOs.Auth;
using HealthCareAB_v1.Exceptions;
using HealthCareAB_v1.Models;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace HealthCareAB_v1.Services.Implementations;

/// <summary>
/// Service handling authentication operations including registration and login.
/// </summary>
public class AuthService(
    IUserService userService,
    IJwtTokenService jwtTokenService,
    IOptions<JwtSettings> jwtSettings,
    IWebHostEnvironment environment
// IHttpContextAccessor httpContextAccessor
) : IAuthService
{
    private readonly IUserService _userService =
        userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly IJwtTokenService _jwtTokenService =
        jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    private readonly JwtSettings _jwtSettings =
        jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
    private readonly bool _isDevelopment = environment?.IsDevelopment() ?? false;

    // private readonly IHttpContextAccessor _httpContextAccessor =
    //     httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    /// <inheritdoc />
    public async Task<AuthResponseDto> RegisterPatientAsync(RegisterPatientDto registerDto)
    {
        ArgumentNullException.ThrowIfNull(registerDto);

        if (await _userService.ExistsByUsernameAsync(registerDto.Username))
        {
            return new AuthResponseDto { Success = false, Message = "Username is already taken" };
        }

        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = _userService.HashPassword(registerDto.Password),
            Roles = [Roles.Patient],
            Patient = new Patient { },
        };

        await _userService.CreateUserAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "User registered successfully",
            Username = user.Username,
            Roles = user.Roles,
        };
    }

    /// <inheritdoc />
    public async Task<AuthResponseDto> RegisterCaregiverAsync(RegisterCaregiverDto registerDto)
    {
        ArgumentNullException.ThrowIfNull(registerDto);

        if (await _userService.ExistsByUsernameAsync(registerDto.Username))
        {
            return new AuthResponseDto { Success = false, Message = "Username is already taken" };
        }

        // Throws an ValidationException if a role that doesn't exist is sent with the dto
        DetermineValidRoles(registerDto.Roles);

        // Determine roles with security check
        var roles = DetermineCaregiverRoles(registerDto.Roles);

        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = _userService.HashPassword(registerDto.Password),
            Roles = roles,
            Caregiver = new Caregiver { },
        };

        await _userService.CreateUserAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "User registered successfully",
            Username = user.Username,
            Roles = user.Roles,
        };
    }

    /// <summary>
    /// Detemines if the roles for a new caregiver are valid.
    /// </summary>
    private static void DetermineValidRoles(List<string> roles)
    {
        if (roles.Count == 0)
        {
            return;
        }

        var rolesToCheck = roles.Select(Roles.IsValidCaregiverRole).ToList();

        for (var i = 0; i < rolesToCheck.Count; i++)
        {
            if (!rolesToCheck[i])
            {
                throw new ValidationException($"Caregiver cannot have role: {roles[i]}.");
            }
        }
    }

    /// <summary>
    /// Determines the roles for a new caregiver.
    /// Default role is set to CAREGIVER.
    /// </summary>
    private static List<string> DetermineCaregiverRoles(List<string> requestedRoles)
    {
        if (requestedRoles == null || requestedRoles.Count == 0)
        {
            return [Roles.Caregiver];
        }

        // Return requested roles (original behavior)
        return requestedRoles;
    }

    /// <inheritdoc />
    public async Task<(AuthResponseDto response, string? token)> LoginAsync(LoginDto loginDto)
    {
        ArgumentNullException.ThrowIfNull(loginDto);

        var user = await _userService.GetUserByUsernameAsync(loginDto.Username);

        if (user == null || !_userService.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return (
                new AuthResponseDto { Success = false, Message = "Invalid username or password" },
                null
            );
        }

        var token = _jwtTokenService.GenerateToken(user);

        return (
            new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Username = user.Username,
                Roles = user.Roles,
            },
            token
        );
    }

    /// <inheritdoc />
    public CookieOptions GetJwtCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !_isDevelopment,
            Path = "/",
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
        };
    }

    /// <inheritdoc />
    public CookieOptions GetClearCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !_isDevelopment,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
        };
    }
}
