namespace Api.Dtos.Bookings;

public class BookingUpdateDto
{
    public DateTime ScheduledStartUtc { get; set; }
    public int DurationMinutes { get; set; } = 60;

    public string ServiceType { get; set; } = "";
    public string? Notes { get; set; }

    public string? Status { get; set; } // allow Confirmed/Cancelled updates
}