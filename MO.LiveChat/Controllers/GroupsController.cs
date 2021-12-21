using System.Diagnostics;
using Apis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MO.LiveChat.Data;
using MO.LiveChat.Interfaces;
using static MO.LiveChat.Interfaces.IGroupApi;

namespace MO.LiveChat.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupsController : ControllerBase, IGroupApi
{
    private readonly ILogger<GroupsController> _logger;
    private readonly LiveChatDbContext dbContext;

    public GroupsController(ILogger<GroupsController> logger, LiveChatDbContext dbContext)
    {
        _logger = logger;
        this.dbContext = dbContext;
    }

    [HttpGet(GetByIdEndpoint)]
    public async Task<GroupResponse> GetById(long id)
    {
        var entity = await dbContext.Groups.FirstOrDefaultAsync(g => g.GroupId == id);
        if (entity == null) return null;
        return new GroupResponse(entity.GroupId, entity.Name);
    }

    [HttpPut(AddNewEndpoint)]
    public async Task<NewGroupResponse> AddNew(NewGroupRequest request)
    {
        var entity = new Data.Group() { Name = request.Name };
        await dbContext.Groups.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return new NewGroupResponse(entity.GroupId, entity.Name);
    }

    [HttpGet(GetAllEndpoint)]
    public async Task<AllGroupsResponse> GetAll()
    {
        var groups = await dbContext.Groups.ToListAsync();
        return new AllGroupsResponse(groups.Select(g=>(g.GroupId, g.Name)).ToList());
    }
}
