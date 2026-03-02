using Api.Dtos.Bookings;
using MediatR;

namespace Api.Application.Bookings;

public sealed record ConfirmBookingCommand(Guid Id) : IRequest<BookingResponseDto>;
