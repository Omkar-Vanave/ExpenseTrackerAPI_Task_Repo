using ExpenseTrackerAPI.DTOs;

namespace ExpenseTrackerAPI.Services;

/// <summary>
/// Service interface for expense operations with validation and business rules.
/// </summary>
public interface IExpenseService
{
    Task<ExpenseResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExpenseResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ExpenseResponseDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<ExpenseResponseDto> AddAsync(CreateExpenseDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExpenseSummaryByCategoryDto>> GetSummaryByCategoryAsync(int? categoryId, CancellationToken cancellationToken = default);
}
