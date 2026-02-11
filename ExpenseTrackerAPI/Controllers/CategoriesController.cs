using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Services;

namespace ExpenseTrackerAPI.Controllers;

/// <summary>
/// API controller for Category management: Create, Read, Update, Delete.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>Get all categories.</summary>
    /// <response code="200">Returns the list of categories.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    /// <summary>Get a category by id.</summary>
    /// <param name="id">Category id.</param>
    /// <response code="200">Returns the category.</response>
    /// <response code="404">Category not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null) return NotFound();
        return Ok(category);
    }

    /// <summary>Create a new category.</summary>
    /// <response code="201">Category created.</response>
    /// <response code="400">Invalid input.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _categoryService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Update an existing category by id.</summary>
    /// <param name="id">Category id.</param>
    /// <response code="200">Category updated.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="404">Category not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> Update(int id, [FromBody] CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _categoryService.UpdateAsync(id, dto, cancellationToken);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Delete a category by id. Fails if the category has linked expenses.</summary>
    /// <param name="id">Category id.</param>
    /// <response code="204">Category deleted.</response>
    /// <response code="400">Category has linked expenses.</response>
    /// <response code="404">Category not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _categoryService.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
