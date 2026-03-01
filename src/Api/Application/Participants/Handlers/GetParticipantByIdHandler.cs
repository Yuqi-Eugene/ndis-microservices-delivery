using Api.Data;
using Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Participants;

public sealed class GetParticipantByIdHandler : IRequestHandler<GetParticipantByIdQuery, Participant>
{
    private readonly AppDbContext _db;

    public GetParticipantByIdHandler(AppDbContext db) => _db = db;

    public async Task<Participant> Handle(GetParticipantByIdQuery request, CancellationToken ct)
        => await _db.Participants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Participant not found.");
}
