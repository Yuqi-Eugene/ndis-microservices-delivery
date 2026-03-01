using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Bookings;

public sealed record GetBookingByIdQuery(Guid Id) : IRequest<Booking>;
