namespace HealthCareAB_v1.DTOs.Booking;

public class BookingResponseDto
{
    public Guid Id { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateOnly Date { get; init; }

    public TimeOnly Start { get; init; }
    public TimeOnly End { get; init; }
}
