namespace Api.Models;

public class Claim
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ServiceDeliveryId { get; set; }

    public decimal Amount { get; set; }

    // Draft -> Submitted -> Paid (后面可扩展)
    public string Status { get; set; } = "Draft";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ServiceDelivery? ServiceDelivery { get; set; }
}