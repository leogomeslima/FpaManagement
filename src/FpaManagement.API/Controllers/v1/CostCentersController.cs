using Asp.Versioning;
using FpaManagement.Application.Commands.CostCenter;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Common;
using FpaManagement.Application.DTOs.CostCenter;
using FpaManagement.Application.Queries.CostCenter;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CostCentersController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Permissions = "ViewCostCenter")]
    [ProducesResponseType(typeof(PaginatedList<CostCenterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<CostCenterDto>>> GetCostCenters(
        [FromQuery] GetAllCostCentersQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Authorize(Permissions = "ViewCostCenter")]
    [ProducesResponseType(typeof(CostCenterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CostCenterDto>> GetCostCenter(Guid id)
    {
        var result = await Mediator.Send(new GetCostCenterByIdQuery { Id = id });

        if (!result.Succeeded)
            return NotFound(result);

        return Ok(result.Data);
    }

    [HttpGet("by-department/{departmentId}")]
    [Authorize(Permissions = "ViewCostCenter")]
    [ProducesResponseType(typeof(List<CostCenterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CostCenterDto>>> GetCostCentersByDepartment(Guid departmentId)
    {
        return Ok(await Mediator.Send(new GetCostCentersByDepartmentQuery { DepartmentId = departmentId }));
    }

    [HttpGet("lookup")]
    [Authorize(Permissions = "ViewCostCenter")]
    [ProducesResponseType(typeof(List<LookupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LookupDto>>> GetCostCenterLookup(
        [FromQuery] GetCostCenterLookupQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpPost]
    [Authorize(Permissions = "CreateCostCenter")]
    [ProducesResponseType(typeof(CostCenterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CostCenterDto>> CreateCostCenter(CreateCostCenterDto data)
    {
        var result = await Mediator.Send(new CreateCostCenterCommand { Data = data });

        if (!result.Succeeded)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetCostCenter), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Permissions = "EditCostCenter")]
    [ProducesResponseType(typeof(CostCenterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CostCenterDto>> UpdateCostCenter(Guid id, UpdateCostCenterDto data)
    {
        var result = await Mediator.Send(new UpdateCostCenterCommand { Id = id, Data = data });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Permissions = "DeleteCostCenter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCostCenter(Guid id)
    {
        var result = await Mediator.Send(new DeleteCostCenterCommand { Id = id });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}
