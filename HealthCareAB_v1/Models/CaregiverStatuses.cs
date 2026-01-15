namespace HealthCareAB_v1.Models;

public class CaregiverStatuses
{
    public const string Available = "AVAILABLE";
    public const string Unavailable = "UNAVAILABLE";

    public static bool IsValidStatus(string status)
    {
        return status is Available or Unavailable;
    }

    public ICollection<string> CaregiverDailySchedules { get; set; } = [Available, Unavailable];
}
