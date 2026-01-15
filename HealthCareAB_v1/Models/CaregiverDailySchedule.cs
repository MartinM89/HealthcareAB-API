using System.Text.Json.Serialization;

namespace HealthCareAB_v1.Models;

public class CaregiverDailySchedule
{
    public Guid Id { get; set; }

    public Guid CaregiverUserId { get; set; }

    public Guid CaregiverStatusId { get; set; }

    public Guid CaregiverStatusId { get; set; }
    public CaregiverStatus CaregiverStatus { get; set; } = null!;

    [JsonIgnore]
    public Caregiver Caregiver { get; set; } = null!;

    [JsonIgnore]
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public ICollection<Booking> Bookings { get; set; } = [];
}
