using System.Linq;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Core.Repositories;

namespace ExpenseTracker.Core.Services;

public sealed class ExpenseService
{
    private readonly ITransactionRepository _txRepo;
    private readonly ICategoryRepository _catRepo;

    public ExpenseService(ITransactionRepository txRepo, ICategoryRepository catRepo)
    {
        _txRepo = txRepo;
        _catRepo = catRepo;
    }

    public async Task AddExpenseAsync(decimal amount, DateOnly date, string categoryName, string note = "")
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be > 0", nameof(amount));

        var cat = await _catRepo.EnsureAsync(categoryName, CategoryType.Expense);

        var tx = new Transaction
        {
            Amount = amount,
            Date = date,
            CategoryId = cat.CategoryId,
            Note = note,
            Type = TransactionType.Expense
        };

        await _txRepo.AddAsync(tx);
        await _txRepo.SaveAsync();
    }

    public async Task<Dictionary<string, decimal>> TotalsByCategoryAsync(DateOnly? from = null, DateOnly? to = null)
    {
        var all = await _txRepo.GetAllAsync();
        var cats = await _catRepo.GetAllAsync();
        var lookup = cats.ToDictionary(c => c.CategoryId, c => c.Name);

        var filtered = all
            .Where(t => t.Type == TransactionType.Expense)
            .Where(t => (!from.HasValue || t.Date >= from) &&
                        (!to.HasValue || t.Date <= to));

        return filtered
            .GroupBy(t => lookup[t.CategoryId])
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));
    }

    public async Task<decimal> TotalSpentAsync(DateOnly? from = null, DateOnly? to = null)
    {
        var all = await _txRepo.GetAllAsync();

        return all
            .Where(t => t.Type == TransactionType.Expense)
            .Where(t => (!from.HasValue || t.Date >= from) &&
                        (!to.HasValue || t.Date <= to))
            .Sum(t => t.Amount);
    }
}
