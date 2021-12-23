using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MO.LiveChat.Data;
using MO.LiveChat.Interfaces;
using static MO.LiveChat.Interfaces.IMessagesApi;
using Message = MO.LiveChat.Data.Message;

namespace MO.LiveChat.Controllers
{
    [Route("[controller]")]
    public class MessagesController : ControllerBase, IMessagesApi
    {
        private readonly LiveChatDbContext context;
        private readonly IMapper mapper;

        public MessagesController(LiveChatDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(GetMessagesFromGroupEndpoint)]
        [ProducesResponseType(typeof(GetMessagesResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GetMessagesResponse>> GetMessages(Guid groupId)
        {
            var entities = await context.Messages.Where(m => m.GroupId == groupId).ToListAsync();

            return Ok(new GetMessagesResponse(groupId,
                entities.Select(e => mapper.Map<IMessagesApi.Message>(e)).ToList()));
        }

        [HttpPut(NewMessageEndpoint)]
        [ProducesResponseType(typeof(NewMessageResponse), 200)]
        public async Task<ActionResult<NewMessageResponse>> NewMessage(NewMessageRequest request)
        {
            Message message = new Message() { GroupId = request.GroupId, ChatUserId = request.UserId, Text = request.Text, CastDate = DateTime.UtcNow };
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
            return Ok(new NewMessageResponse(message.MessageId));
        }
    }
}
