namespace ExpenseTrackerAPI.Models;

/// <summary>
/// Represents a single expense record linked to a category.
/// </summary>
public class Expense
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }

    // Navigation property
    public virtual Category Category { get; set; } = null!;
}
