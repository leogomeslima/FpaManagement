using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.Role;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Role;
using FpaManagement.Application.Queries.Role;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Permissions = "ManageRoles")]
public class RolesController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<RoleDto>>> GetRoles([FromQuery] GetRolesQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetRole(Guid id)
    {
        var result = await Mediator.Send(new GetRoleByIdQuery { Id = id });
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result.Data);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto data)
    {
        var result = await Mediator.Send(new CreateRoleCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetRole), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> UpdateRole(Guid id, UpdateRoleDto data)
    {
        var result = await Mediator.Send(new UpdateRoleCommand { Id = id, Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var result = await Mediator.Send(new DeleteRoleCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }
}
