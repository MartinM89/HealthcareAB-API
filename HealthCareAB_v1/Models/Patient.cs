using System.ComponentModel.DataAnnotations;

namespace HealthCareAB_v1.Models;

public class Patient
{
    [Key]
    public Guid UserId { get; set; }
    public string SocialSecurityNumber { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public User User { get; set; } = null!;
    public Review? Review { get; set; }
    public ICollection<Booking> Bookings { get; set; } = [];
}
