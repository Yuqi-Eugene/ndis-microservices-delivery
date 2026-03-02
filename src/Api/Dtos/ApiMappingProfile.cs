using Api.Application.Participants;
using Api.Domain.Entities;
using Api.Dtos.Bookings;
using Api.Dtos.Claims;
using Api.Dtos.Participants;
using Api.Dtos.Providers;
using Api.Dtos.ServiceDeliveries;
using AutoMapper;

namespace Api.Dtos;

// AutoMapper profiles define how internal models are projected into outward-facing DTOs.
// Centralizing mappings avoids repetitive manual copying in every handler.
public sealed class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<Participant, ParticipantResponseDto>();
        // Record DTOs with constructor parameters sometimes need explicit constructor mapping.
        CreateMap<ParticipantListResult, ParticipantListResponseDto>()
            .ForCtorParam(nameof(ParticipantListResponseDto.Items), opt => opt.MapFrom(src => src.Items));

        CreateMap<Provider, ProviderResponseDto>();
        CreateMap<Booking, BookingResponseDto>();
        CreateMap<ServiceDelivery, ServiceDeliveryResponseDto>();
        CreateMap<Claim, ClaimResponseDto>();
    }
}
