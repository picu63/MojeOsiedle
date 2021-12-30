using Microsoft.AspNetCore.Mvc;

namespace MO.LiveChat.Interfaces;

public interface IMessagesApi
{
    const string GetMessagesFromGroupEndpoint = "group/{groupId}";
    Task<ActionResult<GetMessagesResponse>> GetMessages([FromQuery]Guid groupId);
    record GetMessagesResponse(Guid GroupId, List<MessageDto> Messages);
    record MessageDto(Guid MessageId, Guid UserId, string Text, List<ReactionDto> Reactions);
    record ReactionDto(ReactionType ReactionType, Guid UserId);
    enum ReactionType { Like = 0, }

    const string NewMessageEndpoint = "new";
    Task<ActionResult<NewMessageResponse>> NewMessage([FromBody]NewMessageRequest request);
    record NewMessageRequest(string Text, Guid GroupId, Guid UserId);
    record NewMessageResponse(Guid MessageId);
}