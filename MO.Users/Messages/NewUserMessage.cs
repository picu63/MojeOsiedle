using MO.Integration.Messages;

namespace MO.Auth.Messages;

public class NewUserMessage : IntegrationBaseMessage
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
}