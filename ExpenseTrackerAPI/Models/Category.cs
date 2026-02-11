namespace ExpenseTrackerAPI.Models;

/// <summary>
/// Represents an expense category (e.g., Food, Transport, Utilities).
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property for related expenses
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
