using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.DTOs.User;

[ExcludeFromCodeCoverage]
public class UserDto
{
    public required string Username { get; set; }
    public required List<string> Roles { get; set; }
}
