namespace HealthCareAB_v1.DTOs.User.Caregiver;

public class CaregiverSchedulesDto
{
    public required Guid CaregiverId { get; set; }
    public required int DaysAhead { get; set; } = 30;
}
