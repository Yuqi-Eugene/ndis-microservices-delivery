using Microsoft.AspNetCore.SignalR;

namespace Api.Models;

public class Provider
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "";
    public string? Abn { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }