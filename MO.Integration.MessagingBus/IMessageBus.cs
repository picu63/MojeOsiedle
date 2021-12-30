using MO.Integration.Messages;

namespace MO.Integration.MessagingBus;

public interface IMessageBus
{
    Task PublishMessage (IntegrationBaseMessage message, string topicName);
}