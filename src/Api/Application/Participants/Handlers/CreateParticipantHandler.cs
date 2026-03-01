using Api.Data;
using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Participants;

public sealed class CreateParticipantHandler : IRequestHandler<CreateParticipantCommand, Participant>
{
    private readonly AppDbContext _db;

    public CreateParticipantHandler(AppDbContext db) => _db = db;

    public async Task<Participant> Handle(CreateParticipantCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        var entity = new Participant
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName.Trim(),
            NdisNumber = dto.NdisNumber?.Trim(),
            Phone = dto.Phone?.Trim(),
            Email = dto.Email?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.Participants.Add(entity);
        await _db.SaveChangesAsync(ct);

        return entity;
    }
}
