using Microsoft.EntityFrameworkCore;
using MO.LiveChat.Data;

namespace MO.LiveChat.Services;

public class ChatUserService : IChatUserService
{
    private readonly LiveChatDbContext context;
    private readonly ILogger<ChatUserService> logger;

    public ChatUserService(LiveChatDbContext context, ILogger<ChatUserService> logger)
    {
        this.context = context;
        this.logger = logger;
    }
    public async Task AddUser(Guid guid, string userName)
    {
        if (await context.ChatUsers.FirstOrDefaultAsync(u=>u.ChatUserId == guid) is not null)
        {
            logger.LogInformation($"Cannot add user with guid: {guid}");
            return;
        }
        await context.ChatUsers.AddAsync(new ChatUser() { ChatUserId = guid, Messages = new List<Message>(), Reactions = new List<Reaction>(), Username = userName });
        await context.SaveChangesAsync();
    }
}