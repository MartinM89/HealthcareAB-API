namespace HealthCareAB_v1.Models;

public class CaregiverStatus
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "UNAVAILABLE";

    public ICollection<CaregiverDailySchedule> CaregiverDailySchedules { get; set; } = [];
}
