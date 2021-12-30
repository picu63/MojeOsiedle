using MO.Integration.Messages;

namespace MO.Auth.Messages;

public class NewUserMessage : IntegrationBaseMessage
{
    public string Username { get; set; }
}