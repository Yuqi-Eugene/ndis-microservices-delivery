using Api.Dtos.Bookings;
using MediatR;

namespace Api.Application.Bookings;

public sealed record UpdateBookingCommand(Guid Id, BookingUpdateDto Dto) : IRequest<BookingResponseDto>;
