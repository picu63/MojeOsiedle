using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using MO.Integration.MessagingBus;
using MO.LiveChat.Configs;
using MO.LiveChat.Data;
using MO.LiveChat.Messages;
using MO.LiveChat.Services;
using Newtonsoft.Json;

namespace MO.LiveChat.Worker;

public class UserUpdater : IHostedService
{
    private readonly ILogger<UserUpdater> logger;
    private readonly IServiceProvider serviceProvider;
    private ServiceBusClient client;
    private ServiceBusProcessor processor;
    private readonly UserUpdaterConfiguration userUpdaterConfiguration;
    private readonly string connectionString;

    public UserUpdater(ILogger<UserUpdater> logger, IServiceProvider serviceProvider, IOptions<UserUpdaterConfiguration> userUpdaterConfiguration, IConfiguration configuration)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.userUpdaterConfiguration = userUpdaterConfiguration.Value;
        this.connectionString = configuration.GetSection("ServiceBusConnectionString").Value;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        client = new ServiceBusClient(connectionString);
        processor = client.CreateProcessor(userUpdaterConfiguration.TopicName,
            userUpdaterConfiguration.SubscriptionName);
        
        // handler to process messages
        processor.ProcessMessageAsync += MessageHandler;

        // handler to process any errors
        processor.ProcessErrorAsync += ErrorHandler;

        try
        {
            await processor.StartProcessingAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error with service bus processor");
            await processor.StopProcessingAsync(cancellationToken);
            throw;
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        logger.LogError(arg.Exception, "Error from service bus listener.");
        return Task.CompletedTask;
    }

    private async Task MessageHandler(ProcessMessageEventArgs arg)
    {
        var messageBody = Encoding.UTF8.GetString(arg.Message.Body);
        var newUserRequestMessage = JsonConvert.DeserializeObject<NewUserRequestMessage>(messageBody);

        if(newUserRequestMessage is null) return;
        var chatUserService = serviceProvider.CreateScope().ServiceProvider.GetService<IChatUserService>();
        if (chatUserService is null) throw new Exception($"Cannot get {nameof(IChatUserService)}");
        await chatUserService.AddUser(newUserRequestMessage.UserId, newUserRequestMessage.Username);

        logger.LogDebug($"{newUserRequestMessage.UserId}: {nameof (UserUpdater)} received item.");
        await Task.Delay(TimeSpan.FromSeconds(userUpdaterConfiguration.UpdaterTimeSpanInSec), arg.CancellationToken);
        logger.LogDebug($"{newUserRequestMessage.UserId}:  {nameof (UserUpdater)} processed item.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug($"{nameof(UserUpdater)} stopping.");
        await processor.DisposeAsync();
        await client.DisposeAsync();
    }
}