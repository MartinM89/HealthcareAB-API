namespace HealthCareAB_v1.Models;

public class Patient : User
{
    public string SocialSecurityNumber { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public Review? Review { get; set; }
    public ICollection<Booking> Bookings { get; set; } = [];
}
