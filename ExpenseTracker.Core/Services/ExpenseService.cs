using System.Collections.Generic;
using System.Linq;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Core.Repositories;

namespace ExpenseTracker.Core.Services;

public sealed class ExpenseService
{
    private readonly ITransactionRepository _txRepo;
    private readonly ICategoryRepository _catRepo;

    public ExpenseService(ICategoryRepository catRepo, ITransactionRepository txRepo)
    {
        _catRepo = catRepo;
        _txRepo = txRepo;
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

    public async Task AddCategoryAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required", nameof(name));

        await _catRepo.EnsureAsync(name.Trim(), CategoryType.Expense);
    }

    public Task<IReadOnlyList<Category>> GetCategoriesAsync()
    {
        return _catRepo.GetAllAsync();
    }

    public async Task<IReadOnlyList<Transaction>> GetExpensesAsync(DateOnly? from = null, DateOnly? to = null)
    {
        var all = await _txRepo.GetAllAsync();
        return all
            .Where(t => t.Type == TransactionType.Expense)
            .Where(t => (!from.HasValue || t.Date >= from) &&
                        (!to.HasValue || t.Date <= to))
            .OrderByDescending(t => t.Date)
            .ToList();
    }

    public IEnumerable<Transaction> GetExpensesByDateRange(DateOnly start, DateOnly end)
    {
        return _txRepo.GetByDateRange(start, end);
    }
    public IEnumerable<Transaction> GetExpensesForToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return GetExpensesByDateRange(today, today);
    }

    public IEnumerable<Transaction> GetExpensesForThisWeek()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        // Monday as start of the week
        int diff = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
        if (diff < 0) diff += 7;

        var weekStart = today.AddDays(-diff);
        var weekEnd = weekStart.AddDays(6);

        return GetExpensesByDateRange(weekStart, weekEnd);
    }

    public IEnumerable<Transaction> GetExpensesForThisMonth()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        return GetExpensesByDateRange(monthStart, monthEnd);
    }
}
