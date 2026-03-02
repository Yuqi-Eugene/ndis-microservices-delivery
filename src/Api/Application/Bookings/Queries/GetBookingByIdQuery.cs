using Api.Dtos.Bookings;
using MediatR;

namespace Api.Application.Bookings;

public sealed record GetBookingByIdQuery(Guid Id) : IRequest<BookingResponseDto>;
