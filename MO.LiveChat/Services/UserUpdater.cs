using Apis;

namespace MO.LiveChat.Services;

public class UserUpdater : BackgroundService
{
    private readonly UserService userService;
    private readonly ChatUsersService chatUsersService;

    public UserUpdater(UserService userService, ChatUsersService chatUsersService)
    {
        this.userService = userService;
        this.chatUsersService = chatUsersService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
    }
}