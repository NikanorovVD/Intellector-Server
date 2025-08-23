using AutoMapper;
using DataLayer.Entities;
using ServiceLayer.Models;
using Shared.Models;

namespace MatchMaking.Automapper
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            // Data - Service
            CreateMap<Lobby, LobbyDto>();
            CreateMap<TimeControl, TimeControlDto>();

            // Service - Data
            CreateMap<LobbyDto, Lobby>();
            CreateMap<TimeControlDto, TimeControl>();

            // App - Service
            CreateMap<CreateOpenLobbyRequest, LobbyDto>()
                .ForMember(d => d.InvitedId, opt => opt.MapFrom(src => (string?)null))
                .ForMember(d => d.AutoMatch, opt => opt.MapFrom(src => false));

            CreateMap<CreatePrivateLobbyRequest, LobbyDto>()
               .ForMember(d => d.AutoMatch, opt => opt.MapFrom(src => false));

            // Service - App
            CreateMap<LobbyDto, OpenLobbyDto>();
        }
    }
}
