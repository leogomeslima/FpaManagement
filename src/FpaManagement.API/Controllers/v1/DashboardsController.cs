using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.Dashboard;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Dashboard;
using FpaManagement.Application.Queries.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class DashboardsController : ApiControllerBase
{
    [HttpGet("user")]
    [Authorize]
    [ProducesResponseType(typeof(List<DashboardDefinitionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DashboardDefinitionDto>>> GetUserDashboards()
    {
        return Ok(await Mediator.Send(new GetUserDashboardsQuery()));
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(DashboardDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DashboardDefinitionDto>> GetDashboard(Guid id)
    {
        var result = await Mediator.Send(new GetDashboardByIdQuery { Id = id });
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result.Data);
    }

    [HttpGet("widgets")]
    [Authorize]
    [ProducesResponseType(typeof(List<WidgetDefinitionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WidgetDefinitionDto>>> GetAvailableWidgets()
    {
        return Ok(await Mediator.Send(new GetAvailableWidgetsQuery()));
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(DashboardDefinitionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DashboardDefinitionDto>> CreateDashboard(CreateDashboardDto data)
    {
        var result = await Mediator.Send(new CreateDashboardCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetDashboard), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}/layout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLayout(Guid id, UpdateDashboardLayoutDto layout)
    {
        var result = await Mediator.Send(new UpdateDashboardLayoutCommand { DashboardId = id, Layout = layout });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/widgets")]
    [Authorize]
    [ProducesResponseType(typeof(DashboardWidgetDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardWidgetDto>> AddWidget(Guid id, [FromBody] Guid widgetDefinitionId)
    {
        var result = await Mediator.Send(new AddWidgetToDashboardCommand { DashboardId = id, WidgetDefinitionId = widgetDefinitionId });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("{dashboardId}/widgets/{widgetInstanceId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveWidget(Guid dashboardId, Guid widgetInstanceId)
    {
        var result = await Mediator.Send(new RemoveWidgetFromDashboardCommand { DashboardId = dashboardId, WidgetInstanceId = widgetInstanceId });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteDashboard(Guid id)
    {
        var result = await Mediator.Send(new DeleteDashboardCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }
}
