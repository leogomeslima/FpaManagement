using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.Kpi;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Kpi;
using FpaManagement.Application.Queries.Kpi;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class KpiController : ApiControllerBase
{
    [HttpGet("dashboard")]
    [Authorize(Permissions = "ViewKpis")]
    [ProducesResponseType(typeof(List<KpiDefinitionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<KpiDefinitionDto>>> GetDashboard([FromQuery] GetKpiDashboardQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet]
    [Authorize(Permissions = "ViewKpis")]
    [ProducesResponseType(typeof(PaginatedList<KpiDefinitionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<KpiDefinitionDto>>> GetDefinitions([FromQuery] GetKpiDefinitionsQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Authorize(Permissions = "ViewKpis")]
    [ProducesResponseType(typeof(KpiDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KpiDefinitionDto>> GetDefinition(Guid id)
    {
        var result = await Mediator.Send(new GetKpiDefinitionByIdQuery { Id = id });
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Permissions = "ManageKpis")]
    [ProducesResponseType(typeof(KpiDefinitionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<KpiDefinitionDto>> CreateDefinition(CreateKpiDefinitionDto data)
    {
        var result = await Mediator.Send(new CreateKpiDefinitionCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetDefinition), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Permissions = "ManageKpis")]
    [ProducesResponseType(typeof(KpiDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<KpiDefinitionDto>> UpdateDefinition(Guid id, UpdateKpiDefinitionDto data)
    {
        var result = await Mediator.Send(new UpdateKpiDefinitionCommand { Id = id, Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Permissions = "ManageKpis")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteDefinition(Guid id)
    {
        var result = await Mediator.Send(new DeleteKpiDefinitionCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("values")]
    [Authorize(Permissions = "ManageKpis")]
    [ProducesResponseType(typeof(KpiValueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<KpiValueDto>> RecordValue(RecordKpiValueDto data)
    {
        var result = await Mediator.Send(new RecordKpiValueCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpGet("alerts")]
    [Authorize(Permissions = "ViewKpis")]
    [ProducesResponseType(typeof(List<KpiAlertDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<KpiAlertDto>>> GetActiveAlerts()
    {
        return Ok(await Mediator.Send(new GetActiveAlertsQuery()));
    }
}
