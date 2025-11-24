using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ExpenseTracker.Core;
using ExpenseTracker.Core.Persistence.FileSystem;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Services;

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
            Console.WriteLine("TODO: AddExpense not implemented yet.");
            Console.WriteLine("Press Enter to go back to menu.");
            Console.ReadLine();
        }

        private static void ListExpenses(ExpenseService service)
        {
            Console.WriteLine("TODO: ListExpenses not implemented yet.");
            Console.WriteLine("Press Enter to go back to menu.");
            Console.ReadLine();
        }

        private static void AddCategory(ExpenseService service)
        {
            Console.WriteLine("TODO: AddCategory not implemented yet.");
            Console.WriteLine("Press Enter to go back to menu.");
            Console.ReadLine();
        }

        private static void ListCategories(ExpenseService service)
        {
            Console.WriteLine("TODO: ListCategories not implemented yet.");
            Console.WriteLine("Press Enter to go back to menu.");
            Console.ReadLine();
        }
    }
}
