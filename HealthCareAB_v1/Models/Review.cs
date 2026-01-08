namespace HealthCareAB_v1.Models;

public class Review
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Patient Patient { get; set; } = null!;
}
