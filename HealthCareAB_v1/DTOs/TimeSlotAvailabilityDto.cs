namespace HealthCareAB_v1.DTOs;

public class TimeSlotAvailabilityDto
{
    public Guid Id { get; set; }
    public required string Start { get; set; }
    public required string End { get; set; }
    public bool IsAvailable { get; set; }
    public Guid? ScheduleId { get; set; }
}
