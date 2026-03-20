using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.Demand;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Demand;
using FpaManagement.Application.Queries.Demand;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class DemandCurveController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Permissions = "ViewDemand")]
    [ProducesResponseType(typeof(PaginatedList<DemandEntryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<DemandEntryDto>>> GetEntries([FromQuery] GetDemandEntriesQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("analysis")]
    [Authorize(Permissions = "ViewDemand")]
    [ProducesResponseType(typeof(DemandAnalysisDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DemandAnalysisDto>> GetAnalysis([FromQuery] GetDemandAnalysisQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpPost]
    [Authorize(Permissions = "CreateDemand")]
    [ProducesResponseType(typeof(DemandEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DemandEntryDto>> CreateEntry(CreateDemandEntryDto data)
    {
        var result = await Mediator.Send(new CreateDemandEntryCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetEntries), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Permissions = "EditDemand")]
    [ProducesResponseType(typeof(DemandEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DemandEntryDto>> UpdateEntry(Guid id, UpdateDemandEntryDto data)
    {
        var result = await Mediator.Send(new UpdateDemandEntryCommand { Id = id, Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Permissions = "DeleteDemand")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteEntry(Guid id)
    {
        var result = await Mediator.Send(new DeleteDemandEntryCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }
}
