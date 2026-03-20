using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.Scenario;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Scenario;
using FpaManagement.Application.Queries.Scenario;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ScenarioController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Permissions = "ViewScenario")]
    [ProducesResponseType(typeof(PaginatedList<ScenarioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<ScenarioDto>>> GetScenarios([FromQuery] GetScenariosQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Authorize(Permissions = "ViewScenario")]
    [ProducesResponseType(typeof(ScenarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScenarioDto>> GetScenario(Guid id)
    {
        var result = await Mediator.Send(new GetScenarioByIdQuery { Id = id });
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Permissions = "CreateScenario")]
    [ProducesResponseType(typeof(ScenarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScenarioDto>> CreateScenario(CreateScenarioDto data)
    {
        var result = await Mediator.Send(new CreateScenarioCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetScenario), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Permissions = "EditScenario")]
    [ProducesResponseType(typeof(ScenarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScenarioDto>> UpdateScenario(Guid id, UpdateScenarioDto data)
    {
        var result = await Mediator.Send(new UpdateScenarioCommand { Id = id, Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Permissions = "DeleteScenario")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteScenario(Guid id)
    {
        var result = await Mediator.Send(new DeleteScenarioCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/run")]
    [Authorize(Permissions = "ViewScenario")]
    [ProducesResponseType(typeof(ScenarioResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScenarioResultDto>> RunSimulation(Guid id)
    {
        var result = await Mediator.Send(new RunScenarioSimulationCommand { ScenarioId = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }
}
