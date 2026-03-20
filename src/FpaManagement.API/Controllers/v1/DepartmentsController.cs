using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.Department;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Application.DTOs.Department;
using FpaManagement.Application.Queries.Department;
using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class DepartmentsController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Permissions = "ViewDepartment")]
    [ProducesResponseType(typeof(PaginatedList<DepartmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<DepartmentDto>>> GetDepartments(
        [FromQuery] GetAllDepartmentsQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Authorize(Permissions = "ViewDepartment")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(Guid id)
    {
        var result = await Mediator.Send(new GetDepartmentByIdQuery { Id = id });

        if (!result.Succeeded)
            return NotFound(result);

        return Ok(result.Data);
    }

    [HttpGet("lookup")]
    [Authorize(Permissions = "ViewDepartment")]
    [ProducesResponseType(typeof(List<LookupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LookupDto>>> GetDepartmentLookup(
        [FromQuery] GetDepartmentLookupQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpPost]
    [Authorize(Permissions = "CreateDepartment")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto data)
    {
        var result = await Mediator.Send(new CreateDepartmentCommand { Data = data });

        if (!result.Succeeded)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetDepartment), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Permissions = "EditDepartment")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DepartmentDto>> UpdateDepartment(Guid id, UpdateDepartmentDto data)
    {
        var result = await Mediator.Send(new UpdateDepartmentCommand { Id = id, Data = data });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Permissions = "DeleteDepartment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDepartment(Guid id)
    {
        var result = await Mediator.Send(new DeleteDepartmentCommand { Id = id });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/activate")]
    [Authorize(Permissions = "EditDepartment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateDepartment(Guid id)
    {
        var result = await Mediator.Send(new ActivateDepartmentCommand { Id = id });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Permissions = "EditDepartment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateDepartment(Guid id)
    {
        var result = await Mediator.Send(new DeactivateDepartmentCommand { Id = id });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}
