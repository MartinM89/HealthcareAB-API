namespace HealthCareAB_v1.Models;

public class CaregiverStatus
{
    // public Guid Id { get; set; }

    // public string Status { get; set; } = "UNAVAILABLE";
    public const string Available = "AVAILABLE";
    public const string Unavailable = "UNAVAILABLE";

    public static bool IsValidStatus(string status)
    {
        return status == Available || status == Unavailable;
    }

    // public ICollection<CaregiverDailySchedule> CaregiverDailySchedules { get; set; } = [];
    public ICollection<string> CaregiverDailySchedules { get; set; } = [Available, Unavailable];
}
