using ExpenseTracker.Core.Models;
using ExpenseTracker.Core.Repositories;

namespace ExpenseTracker.Core.Persistence.FileSystem;

public sealed class CategoryFileRepository : ICategoryRepository
{
    private readonly string _file;
    private readonly List<Category> _cache;

    public CategoryFileRepository(string dataDirectory)
    {
        _file = Path.Combine(dataDirectory, "categories.json");
        _cache = FileStore
            .LoadOrInitAsync<List<Category>>(_file, SeedDefaults)
            .GetAwaiter()
            .GetResult();
    }

    // Default categories for a fresh install
    private static List<Category> SeedDefaults() => new()
    {
        new Category { Name = "Food",       Type = CategoryType.Expense },
        new Category { Name = "Transport",  Type = CategoryType.Expense },
        new Category { Name = "Utilities",  Type = CategoryType.Expense },
        new Category { Name = "School",     Type = CategoryType.Expense },
        new Category { Name = "Salary",     Type = CategoryType.Income  },
    };

    public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Category> copy = _cache.ToList();
        return Task.FromResult(copy);
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var found = _cache.FirstOrDefault(c => c.CategoryId == id);
        return Task.FromResult(found);
    }

    public async Task<Category> EnsureAsync(string name, CategoryType type, CancellationToken ct = default)
    {
        var existing = _cache.FirstOrDefault(c =>
            c.Type == type &&
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            return existing;
        }

        var cat = new Category
        {
            Name = name,
            Type = type
        };

        _cache.Add(cat);
        await FileStore.SaveAsync(_file, _cache);
        return cat;
    }
}
