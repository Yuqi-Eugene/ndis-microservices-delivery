namespace Api.Dtos.Providers;

public class ProviderUpdateDto
{
    public string Name { get; set; } = "";
    public string? Abn { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Status { get; set; }
}