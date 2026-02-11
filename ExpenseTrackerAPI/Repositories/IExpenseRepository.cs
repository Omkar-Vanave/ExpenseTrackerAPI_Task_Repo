using ExpenseTrackerAPI.Models;

namespace ExpenseTrackerAPI.Repositories;

/// <summary>
/// Repository interface for Expense data access.
/// </summary>
public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<Expense> AddAsync(Expense expense, CancellationToken cancellationToken = default);
    void Remove(Expense expense);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default);
}
