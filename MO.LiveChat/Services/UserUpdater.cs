using Apis;
using Microsoft.Extensions.Options;
using MO.LiveChat.Configs;

namespace MO.LiveChat.Services;

public class UserUpdater : BackgroundService
{
    private readonly ILogger<UserUpdater> logger;
    private readonly UserService userService;
    private readonly ChatUsersService chatUsersService;
    private readonly IOptionsMonitor<UserUpdaterConfiguration> configuration;

    public UserUpdater(ILogger<UserUpdater> logger, UserService userService, ChatUsersService chatUsersService, IOptionsMonitor<UserUpdaterConfiguration> configuration)
    {
        this.logger = logger;
        this.userService = userService;
        this.chatUsersService = chatUsersService;
        this.configuration = configuration;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"User updater service starts.");
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var userUpdaterConfig = configuration.CurrentValue;
            try
            {
                if (!userUpdaterConfig.Activated)
                    continue;
                logger.LogInformation("Updating users...");
                await UpdateUsers();
                logger.LogInformation("Users updated.");
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

    private async Task UpdateUsers()
    {
        
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("User updater stopping...");
        return base.StopAsync(cancellationToken);
    }
}