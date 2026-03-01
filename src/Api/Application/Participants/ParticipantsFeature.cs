using Api.Data;
using Api.Domain.Entities;
using Api.Dtos;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Participants;

public sealed record GetParticipantsQuery(int Page = 1, int PageSize = 20, string? Q = null) : IRequest<ParticipantListResult>;
public sealed record GetParticipantByIdQuery(Guid Id) : IRequest<Participant>;
public sealed record CreateParticipantCommand(ParticipantCreateDto Dto) : IRequest<Participant>;
public sealed record UpdateParticipantCommand(Guid Id, ParticipantUpdateDto Dto) : IRequest<Participant>;
public sealed record DeleteParticipantCommand(Guid Id) : IRequest<Unit>;

public sealed record ParticipantListResult(int Total, int Page, int PageSize, IReadOnlyList<Participant> Items);

public sealed class GetParticipantsHandler : IRequestHandler<GetParticipantsQuery, ParticipantListResult>
{
    private readonly AppDbContext _db;

    public GetParticipantsHandler(AppDbContext db) => _db = db;

    public async Task<ParticipantListResult> Handle(GetParticipantsQuery request, CancellationToken ct)
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

        return new ParticipantListResult(total, request.Page, request.PageSize, items);
    }
}

public sealed class GetParticipantByIdHandler : IRequestHandler<GetParticipantByIdQuery, Participant>
{
    private readonly AppDbContext _db;

    public GetParticipantByIdHandler(AppDbContext db) => _db = db;

    public async Task<Participant> Handle(GetParticipantByIdQuery request, CancellationToken ct)
        => await _db.Participants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Participant not found.");
}

public sealed class CreateParticipantHandler : IRequestHandler<CreateParticipantCommand, Participant>
{
    private readonly AppDbContext _db;

    public CreateParticipantHandler(AppDbContext db) => _db = db;

    public async Task<Participant> Handle(CreateParticipantCommand request, CancellationToken ct)
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

        return entity;
    }
}

public sealed class UpdateParticipantHandler : IRequestHandler<UpdateParticipantCommand, Participant>
{
    private readonly AppDbContext _db;

    public UpdateParticipantHandler(AppDbContext db) => _db = db;

    public async Task<Participant> Handle(UpdateParticipantCommand request, CancellationToken ct)
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
        return entity;
    }
}

public sealed class DeleteParticipantHandler : IRequestHandler<DeleteParticipantCommand, Unit>
{
    private readonly AppDbContext _db;

    public DeleteParticipantHandler(AppDbContext db) => _db = db;

    public async Task<Unit> Handle(DeleteParticipantCommand request, CancellationToken ct)
    {
        var entity = await _db.Participants.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException("Participant not found.");

        _db.Participants.Remove(entity);
        await _db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

public sealed class GetParticipantsQueryValidator : AbstractValidator<GetParticipantsQuery>
{
    public GetParticipantsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Q)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Q));
    }
}

public sealed class GetParticipantByIdQueryValidator : AbstractValidator<GetParticipantByIdQuery>
{
    public GetParticipantByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ParticipantId is required.");
    }
}

public sealed class CreateParticipantCommandValidator : AbstractValidator<CreateParticipantCommand>
{
    public CreateParticipantCommandValidator()
    {
        RuleFor(x => x.Dto.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.NdisNumber)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.NdisNumber));
        RuleFor(x => x.Dto.Phone)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Phone));
        RuleFor(x => x.Dto.Email)
            .EmailAddress()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Email));
    }
}

public sealed class UpdateParticipantCommandValidator : AbstractValidator<UpdateParticipantCommand>
{
    public UpdateParticipantCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ParticipantId is required.");
        RuleFor(x => x.Dto.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.NdisNumber)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.NdisNumber));
        RuleFor(x => x.Dto.Phone)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Phone));
        RuleFor(x => x.Dto.Email)
            .EmailAddress()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Email));

        RuleFor(x => x.Dto.Status)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.Status));
    }
}

public sealed class DeleteParticipantCommandValidator : AbstractValidator<DeleteParticipantCommand>
{
    public DeleteParticipantCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ParticipantId is required.");
    }
}
