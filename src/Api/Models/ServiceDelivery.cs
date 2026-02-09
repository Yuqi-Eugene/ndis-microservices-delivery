namespace Api.Models;

public class ServiceDelivery
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BookingId { get; set; }

    public DateTime ActualStartUtc { get; set; }
    public int ActualDurationMinutes { get; set; } = 60;

    public string Status { get; set; } = "Draft"; // Draft -> Submitted

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Booking? Booking { get; set; }
}