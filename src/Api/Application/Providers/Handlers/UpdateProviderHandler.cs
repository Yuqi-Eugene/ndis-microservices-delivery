using Api.Data;
using Api.Dtos.Providers;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Providers;

public sealed class UpdateProviderHandler : IRequestHandler<UpdateProviderCommand, ProviderResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UpdateProviderHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProviderResponseDto> Handle(UpdateProviderCommand request, CancellationToken ct)
    {
        var entity = await _db.Providers.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Provider not found.");

        var dto = request.Dto;

        entity.Name = dto.Name.Trim();
        entity.Abn = dto.Abn?.Trim();
        entity.ContactEmail = dto.ContactEmail?.Trim();
        entity.ContactPhone = dto.ContactPhone?.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Status))
            entity.Status = dto.Status.Trim();

        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<ProviderResponseDto>(entity);
    }
}
