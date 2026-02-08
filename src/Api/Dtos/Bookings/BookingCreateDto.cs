namespace Api.Dtos.Bookings;

public class BookingCreateDto
{
    public Guid ParticipantId { get; set; }
    public Guid ProviderId { get; set; }

    public DateTime ScheduledStartUtc { get; set; }
    public int DurationMinutes { get; set; } = 60;

    public string ServiceType { get; set; } = "";
    public string? Notes { get; set; }
}