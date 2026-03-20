using Asp.Versioning;
using FpaManagement.Application.Commands.Budget;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.Budget;
using FpaManagement.Application.Queries.Budget;

using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BudgetsController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Permissions = "ViewBudget")]
    [ProducesResponseType(typeof(PaginatedList<BudgetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<BudgetDto>>> GetBudgets(
        [FromQuery] GetAllBudgetsQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [Authorize(Permissions = "ViewBudget")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BudgetDto>> GetBudget(Guid id)
    {
        var result = await Mediator.Send(new GetBudgetByIdQuery { Id = id });

        if (!result.Succeeded)
            return NotFound(result);

        return Ok(result.Data);
    }

    [HttpGet("{budgetId}/versions")]
    [Authorize(Permissions = "ViewBudgetVersions")]
    [ProducesResponseType(typeof(List<BudgetVersionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BudgetVersionDto>>> GetBudgetVersions(Guid budgetId)
    {
        return Ok(await Mediator.Send(new GetBudgetVersionsQuery { BudgetId = budgetId }));
    }

    [HttpPost]
    [Authorize(Permissions = "CreateBudget")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BudgetDto>> CreateBudget(CreateBudgetDto data)
    {
        var result = await Mediator.Send(new CreateBudgetCommand { Data = data });

        if (!result.Succeeded)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetBudget), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPost("items")]
    [Authorize(Permissions = "EditBudget")]
    [ProducesResponseType(typeof(BudgetItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BudgetItemDto>> AddBudgetItem(CreateBudgetItemDto data)
    {
        var result = await Mediator.Send(new AddBudgetItemCommand { Data = data });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result.Data);
    }

    [HttpPost("{id}/submit")]
    [Authorize(Permissions = "EditBudget")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitBudgetForApproval(Guid id)
    {
        var result = await Mediator.Send(new SubmitBudgetForApprovalCommand { BudgetId = id });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Permissions = "ApproveBudget")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveBudget(Guid id, [FromBody] string? comments)
    {
        var result = await Mediator.Send(new ApproveBudgetCommand { BudgetId = id, Comments = comments });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/reject")]
    [Authorize(Permissions = "ApproveBudget")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectBudget(Guid id, [FromBody] string reason)
    {
        var result = await Mediator.Send(new RejectBudgetCommand { BudgetId = id, Reason = reason });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("items/{itemId}")]
    [Authorize(Permissions = "EditBudget")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBudgetItem(Guid itemId)
    {
        var result = await Mediator.Send(new DeleteBudgetItemCommand { ItemId = itemId });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}
