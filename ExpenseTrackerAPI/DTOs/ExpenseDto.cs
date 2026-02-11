using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs;

/// <summary>
/// DTO for creating an expense.
/// </summary>
public class CreateExpenseDto
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime ExpenseDate { get; set; }
}

/// <summary>
/// DTO for expense response (includes category name for convenience).
/// </summary>
public class ExpenseResponseDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
}

/// <summary>
/// DTO for total expenses per category (e.g. for charts).
/// </summary>
public class ExpenseSummaryByCategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
