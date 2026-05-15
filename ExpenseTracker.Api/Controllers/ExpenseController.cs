using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("expenses")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Expense>>> GetAll([FromQuery] ExpenseQueryOptions query, CancellationToken cancellationToken)
    {
        var expenses = await _expenseService.GetAllAsync(query, cancellationToken);
        return Ok(expenses);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ExpenseSummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var summary = await _expenseService.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Expense>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var expense = await _expenseService.GetByIdAsync(id, cancellationToken);
        return Ok(expense);
    }

    [HttpPost]
    public async Task<ActionResult<Expense>> Create([FromBody] CreateExpenseDto dto, CancellationToken cancellationToken)
    {
        var expense = await _expenseService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Expense>> Update(Guid id, [FromBody] UpdateExpenseDto dto, CancellationToken cancellationToken)
    {
        var expense = await _expenseService.UpdateAsync(id, dto, cancellationToken);
        return Ok(expense);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _expenseService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}