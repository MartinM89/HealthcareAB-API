namespace HealthCareAB_v1.DTOs;

public class TimeSlotAvailabilityDto
{
    public string Id { get; set; } = string.Empty;
    public string Start { get; set; } = string.Empty;
    public string End { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public Guid? ScheduleId { get; set; }
}