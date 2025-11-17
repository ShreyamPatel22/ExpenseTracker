namespace ExpenseTracker.Core.Models;

public enum TransactionType
{
    Expense = 0,
    Income = 1
}

public class Transaction
{
    public Guid TransactionId { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public string Note { get; set; } = "";
    public Guid CategoryId { get; set; }
    public Guid? AccountId { get; set; }

    // Expense by default
    public TransactionType Type { get; set; } = TransactionType.Expense;
}
