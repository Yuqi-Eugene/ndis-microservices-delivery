using Api.Data;
using Api.Dtos;
using Api.Dtos.Providers;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Providers;

public sealed class GetProvidersHandler : IRequestHandler<GetProvidersQuery, CollectionResponseDto<ProviderResponseDto>>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetProvidersHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CollectionResponseDto<ProviderResponseDto>> Handle(GetProvidersQuery request, CancellationToken ct)
    {
        var items = await _db.Providers.AsNoTracking().ToListAsync(ct);
        var responseItems = _mapper.Map<List<ProviderResponseDto>>(items);
        return new CollectionResponseDto<ProviderResponseDto>(responseItems.Count, responseItems);
    }
}
