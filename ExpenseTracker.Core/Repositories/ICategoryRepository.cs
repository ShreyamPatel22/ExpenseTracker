using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Category> EnsureAsync(string name, CategoryType type, CancellationToken ct = default);
}