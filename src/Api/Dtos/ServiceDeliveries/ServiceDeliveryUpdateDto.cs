namespace Api.Dtos.ServiceDeliveries;

public class ServiceDeliveryUpdateDto
{
    public DateTime ActualStartUtc { get; set; }
    public int ActualDurationMinutes { get; set; } = 60;
    public string? Notes { get; set; }
}