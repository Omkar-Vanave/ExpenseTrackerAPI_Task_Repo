using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Models;
using ExpenseTrackerAPI.Repositories;

namespace ExpenseTrackerAPI.Services;

/// <summary>
/// Category service implementing business rules: category cannot be deleted if expenses are linked.
/// Updating category name does not affect existing expenses (only the Category entity is updated).
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return category == null ? null : MapToDto(category);
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(MapToDto);
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var name = (dto.Name ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Category name must not be empty.", nameof(dto.Name));

        var category = new Category { Name = name };
        var created = await _categoryRepository.AddAsync(category, cancellationToken);
        return MapToDto(created);
    }

    public async Task<CategoryResponseDto?> UpdateAsync(int id, CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var name = (dto.Name ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Category name must not be empty.", nameof(dto.Name));

        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null) return null;

        category.Name = name;
        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null) return false;

        // Business rule: Category cannot be deleted if expenses are linked to it.
        if (await _categoryRepository.HasExpensesAsync(id, cancellationToken))
            throw new InvalidOperationException("Category cannot be deleted because it has linked expenses. Remove or reassign expenses first.");

        _categoryRepository.Remove(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CategoryResponseDto MapToDto(Category c) => new() { Id = c.Id, Name = c.Name };
}
