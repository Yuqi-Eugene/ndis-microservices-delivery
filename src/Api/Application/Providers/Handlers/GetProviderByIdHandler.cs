using Api.Data;
using Api.Dtos.Providers;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Providers;

public sealed class GetProviderByIdHandler : IRequestHandler<GetProviderByIdQuery, ProviderResponseDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetProviderByIdHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProviderResponseDto> Handle(GetProviderByIdQuery request, CancellationToken ct)
    {
        var entity = await _db.Providers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Provider not found.");

        return _mapper.Map<ProviderResponseDto>(entity);
    }
}
