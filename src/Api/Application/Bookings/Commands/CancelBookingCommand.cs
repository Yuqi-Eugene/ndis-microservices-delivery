using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Bookings;

public sealed record CancelBookingCommand(Guid Id) : IRequest<Booking>;
