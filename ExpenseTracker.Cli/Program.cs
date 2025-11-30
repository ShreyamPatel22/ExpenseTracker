using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
//using System.Transactions;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Core;
using ExpenseTracker.Core.Persistence.FileSystem;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Services;
using System.Collections.Generic;

namespace ExpenseTracker.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ExpenseTracker");

            Directory.CreateDirectory(dataDir);

            // File-based repositories
            var categoryRepo = new CategoryFileRepository(dataDir);
            var transactionRepo = new TransactionFileRepository(dataDir);

            // Service layer (adjust ctor if your ExpenseService is different)
            var service = new ExpenseService(categoryRepo, transactionRepo);

            RunMainMenu(service);
        }

        private static void RunMainMenu(ExpenseService service)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== Expense Tracker ====");
                Console.WriteLine("1) Add Expense");
                Console.WriteLine("2) List Expenses");
                Console.WriteLine("3) Add Category");
                Console.WriteLine("4) List Categories");
                Console.WriteLine("5) Totals By Category");
                Console.WriteLine("6) Filter expenses by date");
                Console.WriteLine("0) Exit");
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddExpense(service);
                        break;
                    case "2":
                        ListExpenses(service);
                        break;
                    case "3":
                        AddCategory(service);
                        break;
                    case "4":
                        ListCategories(service);
                        break;
                    case "5":
                        ShowTotalsByCategory(service);
                        break;
                    case "6":
                        FilterExpensesByDate(service);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press Enter to continue.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        //TODO: Implement AddExpense, ListExpenses, AddCategory, ListCategories methods
        private static void AddExpense(ExpenseService service)
        {
            Console.Clear();
            Console.WriteLine("==== Add Expense ====");

            // Amount
            decimal amount;
            while (true)
            {
                Console.Write("Amount: ");
                var amountInput = Console.ReadLine();

                if (decimal.TryParse(amountInput, out amount) && amount > 0)
                {
                    break;
                }

                Console.WriteLine("Amount must be a positive number. Try again.");
            }
            // Date
            Console.Write("Date (leave blank for today, or enter e.g. 2025-11-24):");
            var dateInput = Console.ReadLine();
            DateOnly date;

            if (string.IsNullOrWhiteSpace(dateInput))
            {
                date = DateOnly.FromDateTime(DateTime.Today);
            }

            else if (!DateOnly.TryParse(dateInput, out date))
            {
                Console.WriteLine("Invalid date. Using today.");
                date = DateOnly.FromDateTime(DateTime.Today);
            }

            // Category
            string categoryName;
            while (true)
            {
                Console.Write("Category name: ");
                categoryName = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(categoryName))
                {
                    break;
                }
                Console.WriteLine("Category name cannot be empty. Try again.");
            }

            // Note
            Console.Write("Note: ");
            var note = Console.ReadLine() ?? "";

            try
            {
                service.AddExpenseAsync(amount, date, categoryName, note).GetAwaiter().GetResult();
                Console.WriteLine();
                Console.WriteLine("Expense saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to go back to menu.");
            Console.ReadLine();
        }

        private static void ListExpenses(ExpenseService service)
        {
            Console.Clear();
            Console.WriteLine("==== List Expenses ====");
            Console.WriteLine();

            var expenses = service.GetExpensesAsync().GetAwaiter().GetResult();

            if (!expenses.Any())
            {
                Console.WriteLine("No expenses found.");

            }
            else
            {
                foreach (var e in expenses)
                {
                    Console.WriteLine($"{e.Date}: ${e.Amount} - {e.Note}");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Press Enter to go back to menu.");
            Console.ReadLine();
        }

        private static void AddCategory(ExpenseService service)
        {
            Console.Clear();
            Console.WriteLine("==== Add Category ====");
            Console.Write("Category name: ");
            var name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty. Press Enter to go back to menu.");
                Console.ReadLine();
                return;
            }

            try
            {
                service.AddCategoryAsync(name).GetAwaiter().GetResult();
                Console.WriteLine();
                Console.WriteLine($"Category '{name.Trim()}' saved. ");
            }

            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to go back to menu.");
            Console.ReadLine();
        }

        private static void ListCategories(ExpenseService service)
        {
            Console.Clear();
            Console.WriteLine("==== List Categories ====");
            Console.WriteLine();

            var categories = service.GetCategoriesAsync().GetAwaiter().GetResult();

            if (!categories.Any())
            {
                Console.WriteLine("No categories found.");
            }
            else
            {
                foreach (var c in categories.OrderBy(c => c.Name))
                {
                    Console.WriteLine($"- {c.Name}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press Enter to go back to menu.");
            Console.ReadLine();
        }

        private static void ShowTotalsByCategory(ExpenseService service)
        {
            Console.Clear();
            Console.WriteLine("==== Totals By Category ====");
            Console.WriteLine();

            var totals = service.TotalsByCategoryAsync().GetAwaiter().GetResult();

            if (!totals.Any())
            {
                Console.WriteLine("No expenses found.");
            }
            else
            {
                foreach (var pair in totals.OrderBy(p => p.Key))
                {
                    Console.WriteLine($"{pair.Key}: ${pair.Value}");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Press Enter to go back to menu.");
            Console.ReadLine();
        }

        private static void FilterExpensesByDate(ExpenseService service)
        {
            Console.WriteLine();
            Console.WriteLine("==== Filter Expenses By Date ====");
            Console.WriteLine("1. Today");
            Console.WriteLine("2. This Week");
            Console.WriteLine("3. This Month");
            Console.WriteLine("4. Custom Range");
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();
            IEnumerable<Transaction> results;

            switch (choice)
            {
                case "1":
                    results = service.GetExpensesForToday();
                    break;
                case "2":
                    results = service.GetExpensesForThisWeek();
                    break;
                case "3":
                    results = service.GetExpensesForThisMonth();
                    break;
                case "4":
                    var (start, end) = PromptForDateRange();
                    results = service.GetExpensesByDateRange(start, end);
                    break;
                default:
                    Console.WriteLine("Invalid option. Press Enter to go back to menu.");
                    Console.ReadLine();
                    return;
            }
            Console.WriteLine("\nFiltered expenses:\n");
            PrintTransactions(results);
        }

        // Helper to prompt for custom date range
        private static (DateOnly start, DateOnly end) PromptForDateRange()
        {
            while (true)
            {
                Console.Write("Start date (YYYY-MM-DD):");
                var startInput = Console.ReadLine();

                Console.Write("End date (YYYY-MM-DD):");
                var endInput = Console.ReadLine();

                if (!DateOnly.TryParse(startInput, out var start) ||
                    !DateOnly.TryParse(endInput, out var end))
                {
                    Console.WriteLine("Invalid dates. Try again.\n");
                    continue;
                }
                if (end < start)
                {
                    Console.WriteLine("End date must be after start date.\n");
                    continue;
                }
                return (start, end);
            }
        }

        private static void PrintTransactions(IEnumerable<Transaction> transactions)
        {
            if (!transactions.Any())
            {
                Console.WriteLine("No expenses found.");
                Console.WriteLine();
                Console.WriteLine("Press Enter to go back to menu.");
                Console.ReadLine();
                return;
            }

            foreach (var e in transactions.OrderBy(e => e.Date))
            {
                Console.WriteLine($"{e.Date}: ${e.Amount} - {e.Note}");
            }
            Console.WriteLine();
            Console.WriteLine("Press Enter to go back to menu.");
            Console.ReadLine();
        }
    }
}


// private static void AddCategory(ExpenseService service)
// {
//     Console.WriteLine("TODO: AddCategory not implemented yet.");
//     Console.WriteLine("Press Enter to go back to menu.");
//     Console.ReadLine();
// }

// private static void ListCategories(ExpenseService service)
// {
//     Console.WriteLine("TODO: ListCategories not implemented yet.");
//     Console.WriteLine("Press Enter to go back to menu.");
//     Console.ReadLine();
// }

