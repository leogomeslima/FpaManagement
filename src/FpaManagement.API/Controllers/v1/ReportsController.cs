using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.Reports;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Reports;
using FpaManagement.Application.Queries.Reports;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReportsController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Permissions = "ViewReports")]
    [ProducesResponseType(typeof(PaginatedList<ReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<ReportDto>>> GetReports([FromQuery] GetSavedReportsQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Authorize(Permissions = "ViewReports")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportDto>> GetReport(Guid id)
    {
        var result = await Mediator.Send(new GetReportByIdQuery { Id = id });
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result.Data);
    }

    [HttpPost("generate")]
    [Authorize(Permissions = "GenerateReports")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateReport(GenerateReportDto data)
    {
        var result = await Mediator.Send(new GenerateReportCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);

        var contentType = data.Format.ToUpper() == "PDF" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"report_{DateTime.Now:yyyyMMddHHmmss}.{data.Format.ToLower()}";
        return File(result.Data, contentType, fileName);
    }

    [HttpPost]
    [Authorize(Permissions = "GenerateReports")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReportDto>> SaveReport(SaveReportDto data)
    {
        var result = await Mediator.Send(new SaveReportCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetReport), new { id = result.Data?.Id }, result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Permissions = "DeleteReport")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteReport(Guid id)
    {
        var result = await Mediator.Send(new DeleteReportCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }
}
