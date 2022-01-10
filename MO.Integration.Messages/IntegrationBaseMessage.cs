namespace MO.Integration.Messages;

public class IntegrationBaseMessage
{
    public Guid MessageId { get; set; }
    public DateTime CreationDateTime { get; set; } = DateTime.Now;
}