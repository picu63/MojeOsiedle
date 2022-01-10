using AutoMapper;
using MO.Auth.Controllers;
using MO.Auth.Data;
using MO.Auth.Interfaces;
using MO.Auth.Messages;

namespace MO.Auth.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<string, Guid>()
            .ConvertUsing(value => Guid.Parse(value));
        CreateMap<AuthUser, IUsersApi.UserDto>()
            .ForMember(au=>au.UserId, o => o.MapFrom(user => user.Id))
            .ReverseMap();
        CreateMap<AuthUser, NewUserMessage>()
            .ForMember(message=>message.UserId, o => o.MapFrom(l => l.Id)).ReverseMap();
    }
}