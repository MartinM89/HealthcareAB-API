using System.Text.Json.Serialization;

namespace HealthCareAB_v1.Models;

public class CaregiverDailySchedule
{
    public Guid Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public Guid CaregiverId { get; set; }

    public Guid CaregiverStatusId { get; set; }

    [JsonIgnore]
    public Caregiver Caregiver { get; set; } = null!;

    [JsonIgnore]
    public CaregiverStatus CaregiverStatus { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = [];
}
