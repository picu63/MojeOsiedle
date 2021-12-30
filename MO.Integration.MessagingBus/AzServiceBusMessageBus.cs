using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Configuration;
using MO.Integration.Messages;
using Newtonsoft.Json;

namespace MO.Integration.MessagingBus;

public class AzServiceBusMessageBus: IMessageBus
{
    public AzServiceBusMessageBus(IConfiguration configuration)
    {
        this.connectionString = configuration.GetSection("ServiceBusConnectionString").Value;
    }

    private readonly string connectionString;

    public async Task PublishMessage(IntegrationBaseMessage message, string topicName)
    {
        ISenderClient topicClient = new TopicClient(connectionString, topicName);

        var jsonMessage = JsonConvert.SerializeObject(message);
        var serviceBusMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };

        await topicClient.SendAsync(serviceBusMessage);
        Console.WriteLine($"Sent message to {topicClient.Path}");
        await topicClient.CloseAsync();
    }
}