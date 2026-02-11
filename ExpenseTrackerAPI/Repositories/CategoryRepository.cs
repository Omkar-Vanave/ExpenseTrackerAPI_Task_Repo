using Microsoft.EntityFrameworkCore;
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Models;

namespace ExpenseTrackerAPI.Repositories;

/// <summary>
/// Repository implementation for Category CRUD operations.
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
    }

    public void Remove(Category category)
    {
        _context.Categories.Remove(category);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasExpensesAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Expenses.AnyAsync(e => e.CategoryId == categoryId, cancellationToken);
    }
}
