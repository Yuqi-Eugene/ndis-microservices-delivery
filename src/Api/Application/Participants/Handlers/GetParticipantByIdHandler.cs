using Api.Data;
using Api.Dtos.Participants;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Participants;

public sealed class GetParticipantByIdHandler : IRequestHandler<GetParticipantByIdQuery, ParticipantResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetParticipantByIdHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ParticipantResponseDto> Handle(GetParticipantByIdQuery request, CancellationToken ct)
    {
        var entity = await _db.Participants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Participant not found.");

        return _mapper.Map<ParticipantResponseDto>(entity);
    }
}
