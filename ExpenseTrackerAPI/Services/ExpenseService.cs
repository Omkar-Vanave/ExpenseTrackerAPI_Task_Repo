using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Models;
using ExpenseTrackerAPI.Repositories;

namespace ExpenseTrackerAPI.Services; 

/// <summary>
/// Expense service enforcing business rules: amount > 0, expense date not in future, category must exist.
/// </summary>
public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<ExpenseResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, cancellationToken);
        return expense == null ? null : MapToDto(expense);
    }

    public async Task<IEnumerable<ExpenseResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var expenses = await _expenseRepository.GetAllAsync(cancellationToken);
        return expenses.Select(MapToDto);
    }

    public async Task<IEnumerable<ExpenseResponseDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var expenses = await _expenseRepository.GetByCategoryIdAsync(categoryId, cancellationToken);
        return expenses.Select(MapToDto);
    }

    public async Task<ExpenseResponseDto> AddAsync(CreateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        // Business rule: Amount must be greater than zero.
        if (dto.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(dto.Amount));

        // Business rule: Expense date cannot be in the future.
        var today = DateTime.UtcNow.Date;
        var expenseDate = dto.ExpenseDate.Date;
        if (expenseDate > today)
            throw new ArgumentException("Expense date cannot be in the future.", nameof(dto.ExpenseDate));

        // Business rule: Expense must belong to an existing category.
        if (!await _expenseRepository.CategoryExistsAsync(dto.CategoryId, cancellationToken))
            throw new ArgumentException("Category does not exist.", nameof(dto.CategoryId));

        var expense = new Expense
        {
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            ExpenseDate = expenseDate
        };
        var created = await _expenseRepository.AddAsync(expense, cancellationToken);
        return MapToDto(created);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, cancellationToken);
        if (expense == null) return false;

        _expenseRepository.Remove(expense);
        await _expenseRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<ExpenseSummaryByCategoryDto>> GetSummaryByCategoryAsync(int? categoryId, CancellationToken cancellationToken = default)
    {
        var expenses = categoryId.HasValue
            ? await _expenseRepository.GetByCategoryIdAsync(categoryId.Value, cancellationToken)
            : await _expenseRepository.GetAllAsync(cancellationToken);
        return expenses
            .GroupBy(e => new { e.CategoryId, CategoryName = e.Category?.Name ?? "Unknown" })
            .Select(g => new ExpenseSummaryByCategoryDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.CategoryName,
                TotalAmount = g.Sum(e => e.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();
    }

    private static ExpenseResponseDto MapToDto(Expense e) => new()
    {
        Id = e.Id,
        CategoryId = e.CategoryId,
        CategoryName = e.Category?.Name ?? string.Empty,
        Amount = e.Amount,
        ExpenseDate = e.ExpenseDate
    };
}
