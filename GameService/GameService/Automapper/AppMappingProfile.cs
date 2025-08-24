using AutoMapper;
using DataLayer;
using ServiceLayer.Models;
using Shared.Models;

namespace GameService.Automapper
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<CreateGameRequest, StartGameMessage>();
            CreateMap<CreateGameRequest, Game>();
        }
    }
}
