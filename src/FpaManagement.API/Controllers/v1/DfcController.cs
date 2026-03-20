using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.Dfc;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Dfc;
using FpaManagement.Application.Queries.Dfc;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class DfcController : ApiControllerBase
{
    [HttpGet("report")]
    [Authorize(Permissions = "ViewDfc")]
    [ProducesResponseType(typeof(DfcReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DfcReportDto>> GetReport([FromQuery] GetDfcReportQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet]
    [Authorize(Permissions = "ViewDfc")]
    [ProducesResponseType(typeof(PaginatedList<DfcEntryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<DfcEntryDto>>> GetEntries([FromQuery] GetDfcEntriesQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Authorize(Permissions = "ViewDfc")]
    [ProducesResponseType(typeof(DfcEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DfcEntryDto>> GetEntry(Guid id)
    {
        var result = await Mediator.Send(new GetDfcEntryByIdQuery { Id = id });
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Permissions = "CreateDfc")]
    [ProducesResponseType(typeof(DfcEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DfcEntryDto>> CreateEntry(CreateDfcEntryDto data)
    {
        var result = await Mediator.Send(new CreateDfcEntryCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetEntry), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Permissions = "EditDfc")]
    [ProducesResponseType(typeof(DfcEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DfcEntryDto>> UpdateEntry(Guid id, UpdateDfcEntryDto data)
    {
        var result = await Mediator.Send(new UpdateDfcEntryCommand { Id = id, Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Permissions = "DeleteDfc")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteEntry(Guid id)
    {
        var result = await Mediator.Send(new DeleteDfcEntryCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }
}
