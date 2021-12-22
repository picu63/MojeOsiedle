using System.Dynamic;
using Microsoft.AspNetCore.Mvc;

namespace MO.LiveChat.Interfaces;

public interface IGroupsApi
{
    const string GetByIdEndpoint = "{id}";
    Task<ActionResult<GetByIdResponse>> GetById(Guid id);
    record GetByIdResponse(Guid GroupId, string Name);


    const string AddNewEndpoint = "";
    Task<ActionResult<AddNewResponse>> AddNew(AddNewRequest request);
    record AddNewResponse(Guid GroupId, string Name);
    record AddNewRequest(string Name);


    const string GetAllEndpoint = "all";
    Task<ActionResult<GetAllResponse>> GetAll();
    record GetAllResponse(List<Group> Groups);
    record Group(Guid GroupId, string Name);
}