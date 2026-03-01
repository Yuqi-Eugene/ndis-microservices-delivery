using Api.Data;
using Api.Domain.Entities;
using Api.Dtos.Providers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Providers;

public sealed record GetProvidersQuery() : IRequest<List<Provider>>;
public sealed record GetProviderByIdQuery(Guid Id) : IRequest<Provider>;
public sealed record CreateProviderCommand(ProviderCreateDto Dto) : IRequest<Provider>;
public sealed record UpdateProviderCommand(Guid Id, ProviderUpdateDto Dto) : IRequest<Provider>;
public sealed record DeleteProviderCommand(Guid Id) : IRequest<Unit>;

public sealed class GetProvidersHandler : IRequestHandler<GetProvidersQuery, List<Provider>>
{
    private readonly AppDbContext _db;

    public GetProvidersHandler(AppDbContext db) => _db = db;

    public Task<List<Provider>> Handle(GetProvidersQuery request, CancellationToken ct)
        => _db.Providers.AsNoTracking().ToListAsync(ct);
}

public sealed class GetProviderByIdHandler : IRequestHandler<GetProviderByIdQuery, Provider>
{
    private readonly AppDbContext _db;

    public GetProviderByIdHandler(AppDbContext db) => _db = db;

    public async Task<Provider> Handle(GetProviderByIdQuery request, CancellationToken ct)
        => await _db.Providers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Provider not found.");
}

public sealed class CreateProviderHandler : IRequestHandler<CreateProviderCommand, Provider>
{
    private readonly AppDbContext _db;

    public CreateProviderHandler(AppDbContext db) => _db = db;

    public async Task<Provider> Handle(CreateProviderCommand request, CancellationToken ct)
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

        return entity;
    }
}

public sealed class UpdateProviderHandler : IRequestHandler<UpdateProviderCommand, Provider>
{
    private readonly AppDbContext _db;

    public UpdateProviderHandler(AppDbContext db) => _db = db;

    public async Task<Provider> Handle(UpdateProviderCommand request, CancellationToken ct)
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
        return entity;
    }
}

public sealed class DeleteProviderHandler : IRequestHandler<DeleteProviderCommand, Unit>
{
    private readonly AppDbContext _db;

    public DeleteProviderHandler(AppDbContext db) => _db = db;

    public async Task<Unit> Handle(DeleteProviderCommand request, CancellationToken ct)
    {
        var entity = await _db.Providers.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Provider not found.");

        _db.Providers.Remove(entity);
        await _db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

public sealed class GetProviderByIdQueryValidator : AbstractValidator<GetProviderByIdQuery>
{
    public GetProviderByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ProviderId is required.");
    }
}

public sealed class CreateProviderCommandValidator : AbstractValidator<CreateProviderCommand>
{
    public CreateProviderCommandValidator()
    {
        RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Abn)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Abn));
        RuleFor(x => x.Dto.ContactPhone)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.ContactPhone));
        RuleFor(x => x.Dto.ContactEmail)
            .EmailAddress()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.ContactEmail));
    }
}

public sealed class UpdateProviderCommandValidator : AbstractValidator<UpdateProviderCommand>
{
    public UpdateProviderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ProviderId is required.");
        RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Abn)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Abn));
        RuleFor(x => x.Dto.ContactPhone)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.ContactPhone));
        RuleFor(x => x.Dto.ContactEmail)
            .EmailAddress()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.ContactEmail));
        RuleFor(x => x.Dto.Status)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Status));
    }
}

public sealed class DeleteProviderCommandValidator : AbstractValidator<DeleteProviderCommand>
{
    public DeleteProviderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ProviderId is required.");
    }
}
