namespace Api.Models;

public class Participant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = "";

    public string? NdisNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}