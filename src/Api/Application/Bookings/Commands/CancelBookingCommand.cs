using Api.Dtos.Bookings;
using MediatR;

namespace Api.Application.Bookings;

public sealed record CancelBookingCommand(Guid Id) : IRequest<BookingResponseDto>;
