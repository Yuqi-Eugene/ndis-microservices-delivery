namespace Api.Dtos;

public class ParticipantCreateDto
{
    public string FullName { get; set; } = "";
    public string? NdisNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}