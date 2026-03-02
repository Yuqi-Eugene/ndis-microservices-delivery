using Api.Data;
using Api.Dtos;
using Api.Dtos.Claims;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Claims;

public sealed class GetClaimsHandler : IRequestHandler<GetClaimsQuery, CollectionResponseDto<ClaimResponseDto>>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetClaimsHandler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CollectionResponseDto<ClaimResponseDto>> Handle(GetClaimsQuery request, CancellationToken ct)
    {
        var items = await _db.Claims
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);

        var responseItems = _mapper.Map<List<ClaimResponseDto>>(items);
        return new CollectionResponseDto<ClaimResponseDto>(responseItems.Count, responseItems);
    }
}
