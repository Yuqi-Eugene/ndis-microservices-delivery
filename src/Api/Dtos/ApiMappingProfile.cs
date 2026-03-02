using Api.Application.Participants;
using Api.Domain.Entities;
using Api.Dtos.Bookings;
using Api.Dtos.Claims;
using Api.Dtos.Participants;
using Api.Dtos.Providers;
using Api.Dtos.ServiceDeliveries;
using AutoMapper;

namespace Api.Dtos;

public sealed class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<Participant, ParticipantResponseDto>();
        CreateMap<ParticipantListResult, ParticipantListResponseDto>()
            .ForCtorParam(nameof(ParticipantListResponseDto.Items), opt => opt.MapFrom(src => src.Items));

        CreateMap<Provider, ProviderResponseDto>();
        CreateMap<Booking, BookingResponseDto>();
        CreateMap<ServiceDelivery, ServiceDeliveryResponseDto>();
        CreateMap<Claim, ClaimResponseDto>();
    }
}
