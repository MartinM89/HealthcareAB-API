namespace HealthCareAB_v1.DTOs.Caregiver;

public record CreateCaregiverDailyScheduleDto(
    DateOnly Date,
    Guid CaregiverId,
    Guid CaregiverStatusId
);
