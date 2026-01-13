namespace HealthCareAB_v1.DTOs.Auth;

public class AuthResponseDto
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public string? Username { get; set; }
    public List<string> Roles { get; set; } = [];
}
