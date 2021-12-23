using System.Diagnostics;
using MO.Apis;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MO.LiveChat.Data;
using MO.LiveChat.Interfaces;
using static MO.LiveChat.Interfaces.IGroupsApi;

namespace MO.LiveChat.Controllers;

[Route("[controller]")]
public class GroupsController : ControllerBase, IGroupsApi
{
    private readonly ILogger<GroupsController> _logger;
    private readonly LiveChatDbContext dbContext;
    private readonly IMapper mapper;

    public GroupsController(ILogger<GroupsController> logger, LiveChatDbContext dbContext, IMapper mapper)
    {
        _logger = logger;
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    [HttpGet(GetByIdEndpoint)]
    [ProducesResponseType(typeof(IGroupsApi.GetByIdResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IGroupsApi.GetByIdResponse>> GetById(Guid id)
    {
        var entity = await dbContext.Groups.FirstOrDefaultAsync(g => g.GroupId == id);
        if (entity == null) return NotFound();
        return Ok(new IGroupsApi.GetByIdResponse(entity.GroupId, entity.Name));
    }

    [HttpGet(GetAllEndpoint)]
    public async Task<ActionResult<GetAllResponse>> GetAll()
    {
        var groups = await dbContext.Groups.ToListAsync();
        return Ok(new GetAllResponse(groups.Select(g=>mapper.Map<IGroupsApi.Group>(g)).ToList()));
    }

    [HttpPut(AddNewEndpoint)]
    [ProducesResponseType(typeof(AddNewResponse), 200)]
    public async Task<ActionResult<AddNewResponse>> AddNew(AddNewRequest request)
    {
        var entity = new Data.Group() { Name = request.Name };
        await dbContext.Groups.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return Ok(new AddNewResponse(entity.GroupId, entity.Name));
    }
}
