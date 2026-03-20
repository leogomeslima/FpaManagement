using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Audit;
using FpaManagement.Application.Queries.Audit;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Permissions = "ViewAuditLogs")]
public class AuditLogsController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<AuditLogDto>>> GetAuditLogs([FromQuery] GetAuditLogsQuery query)
    {
        return await Mediator.Send(query);
    }
}
