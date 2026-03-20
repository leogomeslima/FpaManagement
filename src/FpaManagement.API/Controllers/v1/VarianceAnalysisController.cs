using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Variance;
using FpaManagement.Application.Queries.Variance;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class VarianceAnalysisController : ApiControllerBase
{
    [HttpGet("budget-vs-actual")]
    [Authorize(Permissions = "ViewVariance")]
    [ProducesResponseType(typeof(VarianceReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VarianceReportDto>> GetBudgetVsActual([FromQuery] GetBudgetVsActualVarianceQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("forecast-vs-actual")]
    [Authorize(Permissions = "ViewVariance")]
    [ProducesResponseType(typeof(VarianceReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VarianceReportDto>> GetForecastVsActual([FromQuery] GetForecastVsActualVarianceQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
