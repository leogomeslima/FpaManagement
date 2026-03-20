using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.Settings;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Settings;
using FpaManagement.Application.Queries.Settings;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Permissions = "ManageSettings")]
public class SettingsController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(SystemSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SystemSettingsDto>> GetSettings()
    {
        var result = await Mediator.Send(new GetSystemSettingsQuery());
        return Ok(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(SystemSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SystemSettingsDto>> UpdateSettings(UpdateSystemSettingsDto data)
    {
        var result = await Mediator.Send(new UpdateSystemSettingsCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }
}
