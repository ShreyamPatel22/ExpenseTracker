namespace ExpenseTracker.Core.Models;

public class Category
{
    public Guid CategoryId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public CategoryType Type { get; set; } = CategoryType.Expense;
}

public enum CategoryType { Expense = 0, Income = 1 }