using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs;

/// <summary>
/// DTO for creating or updating a category.
/// </summary>
public class CreateCategoryDto
{
    [Required(ErrorMessage = "Category name is required.")]
    [MaxLength(200)]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO for category response (get by id / get all).
/// </summary>
public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
