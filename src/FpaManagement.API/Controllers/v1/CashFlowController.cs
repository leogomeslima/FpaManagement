using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.CashFlow;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.CashFlow;
using FpaManagement.Application.Queries.CashFlow;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CashFlowController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Permissions = "ViewCashFlow")]
    [ProducesResponseType(typeof(PaginatedList<CashFlowEntryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<CashFlowEntryDto>>> GetEntries([FromQuery] GetCashFlowEntriesQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("summary")]
    [Authorize(Permissions = "ViewCashFlow")]
    [ProducesResponseType(typeof(CashFlowSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CashFlowSummaryDto>> GetSummary([FromQuery] GetCashFlowSummaryQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("{id}")]
    [Authorize(Permissions = "ViewCashFlow")]
    [ProducesResponseType(typeof(CashFlowEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CashFlowEntryDto>> GetEntry(Guid id)
    {
        var result = await Mediator.Send(new GetCashFlowEntryByIdQuery { Id = id });
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Permissions = "CreateCashFlow")]
    [ProducesResponseType(typeof(CashFlowEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CashFlowEntryDto>> CreateEntry(CreateCashFlowEntryDto data)
    {
        var result = await Mediator.Send(new CreateCashFlowEntryCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetEntry), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Permissions = "EditCashFlow")]
    [ProducesResponseType(typeof(CashFlowEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CashFlowEntryDto>> UpdateEntry(Guid id, UpdateCashFlowEntryDto data)
    {
        var result = await Mediator.Send(new UpdateCashFlowEntryCommand { Id = id, Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Permissions = "DeleteCashFlow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteEntry(Guid id)
    {
        var result = await Mediator.Send(new DeleteCashFlowEntryCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/reconcile")]
    [Authorize(Permissions = "EditCashFlow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReconcileEntry(Guid id)
    {
        var result = await Mediator.Send(new ReconcileCashFlowEntryCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }
}
