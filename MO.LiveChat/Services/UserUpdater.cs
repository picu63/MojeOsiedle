using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MO.Apis;
using MO.LiveChat.Configs;
using MO.LiveChat.Data;
using MO.LiveChat.Interfaces;

namespace MO.LiveChat.Services;

public class UserUpdater : BackgroundService
{
    private readonly ILogger<UserUpdater> logger;
    private readonly AuthService authService;
    private readonly IOptionsMonitor<UserUpdaterConfiguration> configuration;
    private readonly IServiceProvider serviceProvider;

    public UserUpdater(ILogger<UserUpdater> logger, AuthService authService, IOptionsMonitor<UserUpdaterConfiguration> configuration, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.authService = authService;
        this.configuration = configuration;
        this.serviceProvider = serviceProvider;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"User updater service starts.");
        
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(configuration.CurrentValue.UpdaterTimeSpanInSec, stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            var userUpdaterConfig = configuration.CurrentValue;
            try
            {
                if (!userUpdaterConfig.Activated)
                    continue;
                logger.LogInformation("Updating users...");
                var usersAdded = await AddNonExistingUsers();
                logger.LogInformation($"{usersAdded} users was updated.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error occurred while updating users.");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(userUpdaterConfig.UpdaterTimeSpanInSec), stoppingToken);
            }
        }
    }

    private async Task<int> AddNonExistingUsers()
    {
        var context = serviceProvider.CreateScope().ServiceProvider.GetService<LiveChatDbContext>();
        if (context is null)
            throw new Exception($"Cannot resolve {nameof(LiveChatDbContext)}");
        var appUsers = (await authService.GetAllAsync()).Users;
        
        foreach (var appUser in appUsers)
        {
            if (await context.ChatUsers.AnyAsync(c=>c.ChatUserId == appUser.UserId))
                continue;
            var chatUser = new ChatUser() { ChatUserId = appUser.UserId, Username = appUser.Username };
            await context.ChatUsers.AddAsync(chatUser);
        }
        return await context.SaveChangesAsync();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("User updater stopping...");
        return base.StopAsync(cancellationToken);
    }
}