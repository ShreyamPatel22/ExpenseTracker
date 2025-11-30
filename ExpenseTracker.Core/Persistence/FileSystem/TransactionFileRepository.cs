using ExpenseTracker.Core.Models;
using ExpenseTracker.Core.Repositories;

namespace ExpenseTracker.Core.Persistence.FileSystem;

public sealed class TransactionFileRepository : ITransactionRepository
{
    private readonly string _file;
    private readonly List<Transaction> _cache;

    public TransactionFileRepository(string dataDirectory)
    {
        _file = Path.Combine(dataDirectory, "transactions.json");
        _cache = FileStore
            .LoadOrInitAsync<List<Transaction>>(_file, () => new List<Transaction>())
            .GetAwaiter()
            .GetResult();
    }

    public Task AddAsync(Transaction tx, CancellationToken ct = default)
    {
        _cache.Add(tx);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Transaction>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Transaction> copy = _cache.ToList();
        return Task.FromResult(copy);
    }

    public Task SaveAsync(CancellationToken ct = default)
    {
        return FileStore.SaveAsync(_file, _cache);
    }

    public IEnumerable<Transaction> GetByDateRange(DateOnly start, DateOnly end)
    {
        return _cache
            .Where(t => t.Date >= start && t.Date <= end)
            .OrderBy(t => t.Date)
            .ToList();
    }
}
