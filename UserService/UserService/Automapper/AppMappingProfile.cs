using AutoMapper;
using DataLayer.Entities;
using ServiceLayer.Models;
using Shared.Models;

namespace UserService.Automapper
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            AllowNullCollections = true;

            // App - Service
            CreateMap<CreateUserRequest, UserPasswordDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => new RoleDto(r, string.Empty))));

            CreateMap<UpdateUserRequest, UserPasswordDto>()
                .ForMember(
                dest => dest.Roles,
                opt => opt.MapFrom(src => src.Roles == null ? null : src.Roles.Select(r => new RoleDto(r, string.Empty))));

            // Service - App
            CreateMap<UserPasswordDto, UserDto>();

            // Data - Service
            CreateMap<AppUser, UserDto>();
            CreateMap<UserRole, RoleDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name));
        }
    }
}
