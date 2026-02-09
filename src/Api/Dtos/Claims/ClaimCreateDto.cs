namespace Api.Dtos.Claims;

public class ClaimCreateDto
{
    public Guid ServiceDeliveryId { get; set; }
    public decimal Amount { get; set; }
}