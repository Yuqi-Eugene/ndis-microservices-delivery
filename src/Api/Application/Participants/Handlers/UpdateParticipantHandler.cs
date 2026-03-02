using Api.Data;
using Api.Dtos.Participants;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Participants;

public sealed class UpdateParticipantHandler : IRequestHandler<UpdateParticipantCommand, ParticipantResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UpdateParticipantHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ParticipantResponseDto> Handle(UpdateParticipantCommand request, CancellationToken ct)
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
        return _mapper.Map<ParticipantResponseDto>(entity);
    }
}
