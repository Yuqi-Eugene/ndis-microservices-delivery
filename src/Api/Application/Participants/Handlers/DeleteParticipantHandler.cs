using Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Participants;

public sealed class DeleteParticipantHandler : IRequestHandler<DeleteParticipantCommand, Unit>
{
    private readonly AppDbContext _db;

    public DeleteParticipantHandler(AppDbContext db) => _db = db;

    public async Task<Unit> Handle(DeleteParticipantCommand request, CancellationToken ct)
    {
        var entity = await _db.Participants.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Participant not found.");

        _db.Participants.Remove(entity);
        await _db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
