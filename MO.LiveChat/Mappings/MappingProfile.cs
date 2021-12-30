using AutoMapper;
using MO.LiveChat.Data;
using MO.LiveChat.Interfaces;

namespace MO.LiveChat.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Message, IMessagesApi.MessageDto>()
            .ForMember(m=>m.UserId, o => o.MapFrom(src => src.ChatUserId))
            .ReverseMap();

        CreateMap<Reaction, IMessagesApi.ReactionDto>()
            .ForMember(dest => dest.UserId, o => o.MapFrom(src => src.ChatUserId))
            .ReverseMap();

        CreateMap<Group, IGroupsApi.GroupDto>()
            .ReverseMap();
    }
}