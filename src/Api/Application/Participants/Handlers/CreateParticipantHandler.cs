using Api.Data;
using Api.Domain.Entities;
using Api.Dtos.Participants;
using AutoMapper;
using MediatR;

namespace Api.Application.Participants;

public sealed class CreateParticipantHandler : IRequestHandler<CreateParticipantCommand, ParticipantResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CreateParticipantHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ParticipantResponseDto> Handle(CreateParticipantCommand request, CancellationToken ct)
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

        return _mapper.Map<ParticipantResponseDto>(entity);
    }
}
