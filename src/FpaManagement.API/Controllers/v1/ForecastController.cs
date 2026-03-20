using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.Forecast;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Forecast;
using FpaManagement.Application.Queries.Forecast;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ForecastController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Permissions = "ViewForecast")]
    [ProducesResponseType(typeof(PaginatedList<ForecastDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<ForecastDto>>> GetForecasts([FromQuery] GetAllForecastsQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Authorize(Permissions = "ViewForecast")]
    [ProducesResponseType(typeof(ForecastDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ForecastDto>> GetForecast(Guid id)
    {
        var result = await Mediator.Send(new GetForecastByIdQuery { Id = id });
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result.Data);
    }

    [HttpGet("{forecastId}/versions")]
    [Authorize(Permissions = "ViewForecastVersions")]
    [ProducesResponseType(typeof(List<ForecastVersionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ForecastVersionDto>>> GetForecastVersions(Guid forecastId)
    {
        return Ok(await Mediator.Send(new GetForecastVersionsQuery { ForecastId = forecastId }));
    }

    [HttpPost]
    [Authorize(Permissions = "CreateForecast")]
    [ProducesResponseType(typeof(ForecastDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ForecastDto>> CreateForecast(CreateForecastDto data)
    {
        var result = await Mediator.Send(new CreateForecastCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetForecast), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Permissions = "EditForecast")]
    [ProducesResponseType(typeof(ForecastDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ForecastDto>> UpdateForecast(Guid id, UpdateForecastDto data)
    {
        var result = await Mediator.Send(new UpdateForecastCommand { Id = id, Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Permissions = "DeleteForecast")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteForecast(Guid id)
    {
        var result = await Mediator.Send(new DeleteForecastCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/submit")]
    [Authorize(Permissions = "EditForecast")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitForecast(Guid id)
    {
        var result = await Mediator.Send(new SubmitForecastCommand { ForecastId = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Permissions = "ApproveForecast")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveForecast(Guid id)
    {
        var result = await Mediator.Send(new ApproveForecastCommand { ForecastId = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id}/reject")]
    [Authorize(Permissions = "ApproveForecast")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectForecast(Guid id, [FromBody] string reason)
    {
        var result = await Mediator.Send(new RejectForecastCommand { ForecastId = id, Reason = reason });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("items")]
    [Authorize(Permissions = "EditForecast")]
    [ProducesResponseType(typeof(ForecastItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ForecastItemDto>> AddForecastItem(CreateForecastItemDto data)
    {
        var result = await Mediator.Send(new AddForecastItemCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpPut("items/{itemId}")]
    [Authorize(Permissions = "EditForecast")]
    [ProducesResponseType(typeof(ForecastItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ForecastItemDto>> UpdateForecastItem(Guid itemId, UpdateForecastItemDto data)
    {
        data.Id = itemId; // ou passar no comando
        var result = await Mediator.Send(new UpdateForecastItemCommand { Id = itemId, Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("items/{itemId}")]
    [Authorize(Permissions = "EditForecast")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteForecastItem(Guid itemId)
    {
        var result = await Mediator.Send(new DeleteForecastItemCommand { ItemId = itemId });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }
}
