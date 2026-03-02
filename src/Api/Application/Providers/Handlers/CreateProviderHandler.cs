using Api.Data;
using Api.Domain.Entities;
using Api.Dtos.Providers;
using AutoMapper;
using MediatR;

namespace Api.Application.Providers;

public sealed class CreateProviderHandler : IRequestHandler<CreateProviderCommand, ProviderResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CreateProviderHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProviderResponseDto> Handle(CreateProviderCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        var entity = new Provider
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Abn = dto.Abn?.Trim(),
            ContactEmail = dto.ContactEmail?.Trim(),
            ContactPhone = dto.ContactPhone?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.Providers.Add(entity);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<ProviderResponseDto>(entity);
    }
}
