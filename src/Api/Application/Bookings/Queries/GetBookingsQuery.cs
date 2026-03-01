using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Bookings;

public sealed record GetBookingsQuery(
    Guid? ParticipantId = null,
    Guid? ProviderId = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null) : IRequest<List<Booking>>;
