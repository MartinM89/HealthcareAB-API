using System.ComponentModel.DataAnnotations;

namespace HealthCareAB_v1.Models;

public class Caregiver
{
    [Key]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
    // public ICollection<CaregiverDailySchedule> DailySchedules { get; set; } = [];
}
