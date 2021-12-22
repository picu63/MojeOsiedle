using AutoMapper;
using MO.LiveChat.Data;
using MO.LiveChat.Interfaces;

namespace MO.LiveChat.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Message, IMessagesApi.Message>()
            .ForMember(m=>m.UserId, o => o.MapFrom(src => src.UserId))
            .ReverseMap();

        CreateMap<Reaction, IMessagesApi.Reaction>()
            .ForMember(dest => dest.UserId, o => o.MapFrom(src => src.UserId))
            .ReverseMap();

        CreateMap<Group, IGroupsApi.Group>()
            .ReverseMap();
    }
}