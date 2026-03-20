using Asp.Versioning;
using FpaManagement.Application.Commands.User;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Application.DTOs.User;
using FpaManagement.Application.Queries.User;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Permissions = "ManageUsers")]
public class UsersController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<UserDto>>> GetUsers(
        [FromQuery] GetAllUsersQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var result = await Mediator.Send(new GetUserByIdQuery { Id = id });

        if (!result.Succeeded)
            return NotFound(result);

        return Ok(result.Data);
    }

    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<LookupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LookupDto>>> GetUserLookup(
        [FromQuery] GetUserLookupQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto data)
    {
        var result = await Mediator.Send(new CreateUserCommand { Data = data });

        if (!result.Succeeded)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetUser), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, UpdateUserDto data)
    {
        var result = await Mediator.Send(new UpdateUserCommand { Id = id, Data = data });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await Mediator.Send(new DeleteUserCommand { Id = id });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        var result = await Mediator.Send(new ActivateUserCommand { Id = id });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var result = await Mediator.Send(new DeactivateUserCommand { Id = id });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRoles(Guid id, List<Guid> roleIds)
    {
        var result = await Mediator.Send(new AssignUserRolesCommand { UserId = id, RoleIds = roleIds });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}
