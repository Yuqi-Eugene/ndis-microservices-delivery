using Api.Domain.Entities;
using Api.Dtos.Bookings;
using MediatR;

namespace Api.Application.Bookings;

public sealed record CreateBookingCommand(BookingCreateDto Dto) : IRequest<Booking>;
