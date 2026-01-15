namespace HealthCareAB_v1.DTOs.User;

public class UserDto
{
    public required string Username { get; set; }
    public required List<string> Roles { get; set; }
}
