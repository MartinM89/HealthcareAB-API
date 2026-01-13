namespace HealthCareAB_v1.DTOs.Caregiver;

public sealed class CreateCaregiverDailyScheduleDto
{
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }

    public TimeOnly WorkingStartTime { get; init; }
    public TimeOnly WorkingEndTime { get; init; }

    public Guid CaregiverId { get; init; }
    public Guid CaregiverStatusId { get; init; }
}
