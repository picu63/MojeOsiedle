namespace MO.LiveChat.Messages;

public class NewUserRequestMessage
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
}