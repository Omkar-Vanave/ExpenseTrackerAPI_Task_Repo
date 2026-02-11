using Microsoft.EntityFrameworkCore;
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Models;

namespace ExpenseTrackerAPI.Repositories;

/// <summary>
/// Repository implementation for Expense CRUD operations.
/// </summary>
public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _context;

    public ExpenseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .OrderByDescending(e => e.ExpenseDate)
            .ThenBy(e => e.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .Where(e => e.CategoryId == categoryId)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Expense> AddAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(cancellationToken);
        return expense;
    }

    public void Remove(Expense expense)
    {
        _context.Expenses.Remove(expense);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
    }
}
