using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using MO.Integration.Messages;

namespace MO.Integration.MessagingBus;

public class AzServiceBusMessageBus: IMessageBus
{

    // the client that owns the connection and can be used to create senders and receivers
    private static ServiceBusClient client;

    public AzServiceBusMessageBus(string connectionString)
    {
        client = new ServiceBusClient(connectionString);
    }

    public async Task PublishMessage(IntegrationBaseMessage message, string topicName)
    {
        var sender = client.CreateSender(topicName);
        // create a batch 
        using var messageBatch = await sender.CreateMessageBatchAsync();

        if (!messageBatch.TryAddMessage(new ServiceBusMessage(JsonSerializer.Serialize((object)message))))
        {
            // if it is too large for the batch
            throw new Exception($"The message {message.MessageId} is too large to fit in the batch.");
        }
        // Use the producer client to send the batch of messages to the Service Bus queue
        await sender.SendMessagesAsync(messageBatch);
        Console.WriteLine($"A batch of message has been published to the queue.");
    }
}