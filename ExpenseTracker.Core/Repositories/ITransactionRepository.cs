using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Repositories;

public interface ITransactionRepository
{
    Task AddAsync(Transaction tx, CancellationToken ct = default);
    Task<IReadOnlyList<Transaction>> GetAllAsync(CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);

    IEnumerable<Transaction> GetByDateRange(DateOnly start, DateOnly end);
}
