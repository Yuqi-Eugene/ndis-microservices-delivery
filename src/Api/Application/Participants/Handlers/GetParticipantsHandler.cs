using Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Participants;

public sealed class GetParticipantsHandler : IRequestHandler<GetParticipantsQuery, ParticipantListResult>
{
    private readonly AppDbContext _db;

    public GetParticipantsHandler(AppDbContext db) => _db = db;

    public async Task<ParticipantListResult> Handle(GetParticipantsQuery request, CancellationToken ct)
    {
        var query = _db.Participants.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var keyword = request.Q.Trim().ToLower();
            query = query.Where(p =>
                p.FullName.ToLower().Contains(keyword) ||
                (p.Email != null && p.Email.ToLower().Contains(keyword)) ||
                (p.NdisNumber != null && p.NdisNumber.ToLower().Contains(keyword)));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return new ParticipantListResult(total, request.Page, request.PageSize, items);
    }
}
