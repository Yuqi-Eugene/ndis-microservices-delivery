using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Participants;

public sealed class UpdateParticipantHandler : IRequestHandler<UpdateParticipantCommand, Participant>
{
    private readonly AppDbContext _db;

    public UpdateParticipantHandler(AppDbContext db) => _db = db;

    public async Task<Participant> Handle(UpdateParticipantCommand request, CancellationToken ct)
    {
        var entity = await _db.Participants.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Participant not found.");

        var dto = request.Dto;

        entity.FullName = dto.FullName.Trim();
        entity.NdisNumber = dto.NdisNumber?.Trim();
        entity.Phone = dto.Phone?.Trim();
        entity.Email = dto.Email?.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Status))
            entity.Status = dto.Status.Trim();

        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return entity;
    }
}
