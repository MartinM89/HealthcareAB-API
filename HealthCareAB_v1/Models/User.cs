using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareAB_v1.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// User roles for authorization. Defaults to empty list.
    /// Default role is assigned during registration in AuthService.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public List<string> Roles { get; set; } = [];

    public Patient? Patient { get; set; }
    public Caregiver? Caregiver { get; set; }
}
