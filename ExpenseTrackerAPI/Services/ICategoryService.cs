using ExpenseTrackerAPI.DTOs;

namespace ExpenseTrackerAPI.Services;

/// <summary>
/// Service interface for category operations with business rules.
/// </summary>
public interface ICategoryService
{
    Task<CategoryResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<CategoryResponseDto?> UpdateAsync(int id, CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
