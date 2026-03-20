using Asp.Versioning;
using FpaManagement.API.Controllers;
using FpaManagement.Application.Commands.RevenueExpense;
using FpaManagement.Application.Common.Models;
using FpaManagement.Application.Common.Security;
using FpaManagement.Application.DTOs.RevenueExpense;
using FpaManagement.Application.Queries.RevenueExpense;
using Microsoft.AspNetCore.Mvc;

namespace FpaManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class RevenueExpenseController : ApiControllerBase
{
    // Revenue endpoints
    [HttpGet("revenues")]
    [Authorize(Permissions = "ViewTransactions")]
    [ProducesResponseType(typeof(PaginatedList<RevenueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<RevenueDto>>> GetRevenues([FromQuery] GetRevenuesQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("revenues/{id}")]
    [Authorize(Permissions = "ViewTransactions")]
    [ProducesResponseType(typeof(RevenueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RevenueDto>> GetRevenue(Guid id)
    {
        var result = await Mediator.Send(new GetRevenueByIdQuery { Id = id });
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result.Data);
    }

    [HttpPost("revenues")]
    [Authorize(Permissions = "CreateTransaction")]
    [ProducesResponseType(typeof(RevenueDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RevenueDto>> CreateRevenue(CreateRevenueDto data)
    {
        var result = await Mediator.Send(new CreateRevenueCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetRevenue), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("revenues/{id}")]
    [Authorize(Permissions = "EditTransaction")]
    [ProducesResponseType(typeof(RevenueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RevenueDto>> UpdateRevenue(Guid id, UpdateRevenueDto data)
    {
        var result = await Mediator.Send(new UpdateRevenueCommand { Id = id, Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("revenues/{id}")]
    [Authorize(Permissions = "DeleteTransaction")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteRevenue(Guid id)
    {
        var result = await Mediator.Send(new DeleteRevenueCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    // Expense endpoints (similar)
    [HttpGet("expenses")]
    [Authorize(Permissions = "ViewTransactions")]
    [ProducesResponseType(typeof(PaginatedList<ExpenseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<ExpenseDto>>> GetExpenses([FromQuery] GetExpensesQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("expenses/{id}")]
    [Authorize(Permissions = "ViewTransactions")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExpenseDto>> GetExpense(Guid id)
    {
        var result = await Mediator.Send(new GetExpenseByIdQuery { Id = id });
        if (!result.Succeeded)
            return NotFound(result);
        return Ok(result.Data);
    }

    [HttpPost("expenses")]
    [Authorize(Permissions = "CreateTransaction")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExpenseDto>> CreateExpense(CreateExpenseDto data)
    {
        var result = await Mediator.Send(new CreateExpenseCommand { Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetExpense), new { id = result.Data?.Id }, result.Data);
    }

    [HttpPut("expenses/{id}")]
    [Authorize(Permissions = "EditTransaction")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExpenseDto>> UpdateExpense(Guid id, UpdateExpenseDto data)
    {
        var result = await Mediator.Send(new UpdateExpenseCommand { Id = id, Data = data });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("expenses/{id}")]
    [Authorize(Permissions = "DeleteTransaction")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteExpense(Guid id)
    {
        var result = await Mediator.Send(new DeleteExpenseCommand { Id = id });
        if (!result.Succeeded)
            return BadRequest(result);
        return Ok(result);
    }

    // Categories
    [HttpGet("categories")]
    [Authorize(Permissions = "ViewTransactions")]
    [ProducesResponseType(typeof(List<FinancialCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FinancialCategoryDto>>> GetCategories([FromQuery] GetFinancialCategoriesQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpPost("categories")]
    [Authorize(Permissions = "ManageCategories")]
    [ProducesResponseType(typeof(FinancialCategoryDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FinancialCategoryDto>> CreateCategory(CreateFinancialCategoryDto data)
    {
        var result = await Mediator.Send(new CreateFinancialCategoryCommand { Data = data });
        return CreatedAtAction(nameof(GetCategories), new { id = result.Data?.Id }, result.Data);
    }

    // Recurring patterns
    [HttpGet("recurring")]
    [Authorize(Permissions = "ViewTransactions")]
    [ProducesResponseType(typeof(List<RecurringPatternDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RecurringPatternDto>>> GetRecurringPatterns([FromQuery] GetRecurringPatternsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpPost("recurring")]
    [Authorize(Permissions = "CreateTransaction")]
    [ProducesResponseType(typeof(RecurringPatternDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<RecurringPatternDto>> CreateRecurringPattern(CreateRecurringPatternDto data)
    {
        var result = await Mediator.Send(new CreateRecurringPatternCommand { Data = data });
        return CreatedAtAction(nameof(GetRecurringPatterns), new { id = result.Data?.Id }, result.Data);
    }
}
