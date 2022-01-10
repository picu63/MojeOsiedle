namespace MO.LiveChat.Services;

public interface IChatUserService
{
    Task AddUser(Guid guid, string userName);
}