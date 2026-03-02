using Api.Dtos;
using Api.Dtos.Bookings;
using MediatR;

namespace Api.Application.Bookings;

public sealed record GetBookingsQuery(
    Guid? ParticipantId = null,
    Guid? ProviderId = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null) : IRequest<CollectionResponseDto<BookingResponseDto>>;
