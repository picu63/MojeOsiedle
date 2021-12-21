using System.Dynamic;

namespace MO.LiveChat.Interfaces;

public interface IGroupApi
{
    const string GetByIdEndpoint = "{id}";
    Task<GroupResponse> GetById(long id);
    public record GroupResponse(long GroupId, string Name);


    const string AddNewEndpoint = "";
    Task<NewGroupResponse> AddNew(NewGroupRequest request);
    public record NewGroupResponse(long GroupId, string Name);
    public record NewGroupRequest(string Name);


    const string GetAllEndpoint = "all";
    Task<AllGroupsResponse> GetAll();
    public record AllGroupsResponse(List<(long GroupId, string Name)> Groups);
}