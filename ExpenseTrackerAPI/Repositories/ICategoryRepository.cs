using ExpenseTrackerAPI.Models;

namespace ExpenseTrackerAPI.Repositories;

/// <summary>
/// Repository interface for Category data access.
/// </summary>
public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default);
    void Update(Category category);
    void Remove(Category category);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> HasExpensesAsync(int categoryId, CancellationToken cancellationToken = default);
}
