using AutoMapper;
using MO.Auth.Data;
using MO.Auth.Interfaces;

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
    }
}