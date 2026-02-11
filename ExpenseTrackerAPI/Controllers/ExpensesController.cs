using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Services;

namespace ExpenseTrackerAPI.Controllers;

/// <summary>
/// API controller for Expense management: Add, Read, Delete (and get by category).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    /// <summary>Get all expenses.</summary>
    /// <response code="200">Returns the list of expenses.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExpenseResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExpenseResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var expenses = await _expenseService.GetAllAsync(cancellationToken);
        return Ok(expenses);
    }

    /// <summary>Get an expense by id.</summary>
    /// <param name="id">Expense id.</param>
    /// <response code="200">Returns the expense.</response>
    /// <response code="404">Expense not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExpenseResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExpenseResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var expense = await _expenseService.GetByIdAsync(id, cancellationToken);
        if (expense == null) return NotFound();
        return Ok(expense);
    }

    /// <summary>Get all expenses for a given category.</summary>
    /// <param name="categoryId">Category id.</param>
    /// <response code="200">Returns the list of expenses for the category.</response>
    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ExpenseResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExpenseResponseDto>>> GetByCategory(int categoryId, CancellationToken cancellationToken)
    {
        var expenses = await _expenseService.GetByCategoryIdAsync(categoryId, cancellationToken);
        return Ok(expenses);
    }

    /// <summary>Get total expenses grouped by category (for charts). Optional categoryId filter.</summary>
    /// <param name="categoryId">Optional category id to filter summary to a single category.</param>
    /// <response code="200">Returns summary of total amount per category.</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(IEnumerable<ExpenseSummaryByCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExpenseSummaryByCategoryDto>>> GetSummary([FromQuery] int? categoryId, CancellationToken cancellationToken)
    {
        var summary = await _expenseService.GetSummaryByCategoryAsync(categoryId, cancellationToken);
        return Ok(summary);
    }

    /// <summary>Add a new expense. Amount must be &gt; 0, date cannot be in the future, category must exist.</summary>
    /// <response code="201">Expense created.</response>
    /// <response code="400">Invalid input or business rule violation.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExpenseResponseDto>> Add([FromBody] CreateExpenseDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _expenseService.AddAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message, param = ex.ParamName });
        }
    }

    /// <summary>Delete an expense by id.</summary>
    /// <param name="id">Expense id.</param>
    /// <response code="204">Expense deleted.</response>
    /// <response code="404">Expense not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _expenseService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
