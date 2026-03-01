using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Bookings;

public sealed record ConfirmBookingCommand(Guid Id) : IRequest<Booking>;
