namespace HealthCareAB_v1.DTOs.Booking.CaregiverScheduleDtos;

public class PatientInfoDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";
}
