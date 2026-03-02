using Api.Data;
using Api.Dtos.Participants;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Participants;

public sealed class GetParticipantsHandler : IRequestHandler<GetParticipantsQuery, ParticipantListResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetParticipantsHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ParticipantListResponseDto> Handle(GetParticipantsQuery request, CancellationToken ct)
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

        return _mapper.Map<ParticipantListResponseDto>(
            new ParticipantListResult(total, request.Page, request.PageSize, items));
    }
}
