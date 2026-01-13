namespace HealthCareAB_v1.DTOs.Caregiver;

public record CreateCaregiverDailyScheduleDto(
    DateTime StartTime,
    DateTime EndTime,
    Guid CaregiverId,
    Guid CaregiverStatusId,
    Guid BookingId
);
