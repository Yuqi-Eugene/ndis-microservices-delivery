namespace Api.Models;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ParticipantId { get; set; }
    public Guid ProviderId { get; set; }

    public DateTime ScheduledStartUtc { get; set; }
    public int DurationMinutes { get; set; } = 60;

    // Example: "Support Work", "Community Access"
    public string ServiceType { get; set; } = "";

    // Draft -> Confirmed -> Cancelled
    public string Status { get; set; } = "Draft";

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation (optional but useful)
    public Participant? Participant { get; set; }
    public Provider? Provider { get; set; }
}