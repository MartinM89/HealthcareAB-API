namespace HealthCareAB_v1.DTOs.Booking.CaregiverScheduleDtos;

public class BookingDto
{
    public Guid Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateOnly Date { get; set; }

    public PatientInfoDto Patient { get; set; } = null!;

    public TimeSlotDto TimeSlot { get; set; } = null!;
}
