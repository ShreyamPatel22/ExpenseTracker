# ExpenseTracker  
A simple C# console application for tracking expenses, managing categories, and generating summaries.  
Built for ITCS 3112 – Software Engineering & Design.

---

## Overview
ExpenseTracker is a lightweight command-line tool that allows users to:

- Add expenses with amount, date, category, and notes  
- List all expenses  
- Create and view categories  
- View totals grouped by category  
- Filter expenses by:
  - Today  
  - This Week  
  - This Month  
  - Custom Date Range  
- Persist all data using JSON files  

### Architecture:
CLI -> Services -> Repositories -> JSON File Storage

### Requirements
- .NET SDK 8.0 or 9.0  
- Windows, macOS, or Linux terminal

### Steps to Run
Clone the repository:

git clone https://github.com/ShreyamPatel22/ExpenseTracker.git

cd ExpenseTracker

Build the project:
dotnet build

Run the application:
dotnet run --project ExpenseTracker.cli

Data Storage Location
The program stores its data automatically in your system's AppData directory:
AppData/Roaming/ExpenseTracker/


Files created:
transactions.json
categories.json

```
### Main Menu
1) Add Expense
2) List Expenses
3) Add Category
4) List Categories
5) Totals By Category
6) Filter expenses by date
0) Exit
```

1) Example Output
Filtered expenses example:
11/24/2025: $300 – Too much
11/26/2025: $200 – hobbies
11/29/2025: $100 – fish grill

Totals by category example:
Food: $120.50
Shopping: $89.99
Travel: $300.00


Project Structure
ExpenseTracker.sln
|
|- ExpenseTracker.Cli/          # Console UI
|- ExpenseTracker.Core/         # Models, repositories, services
|- ExpenseTracker.Tests/        # Unit tests (if implemented)
|_ExpenseTracker.Web/          # Placeholder project


### Features Summary
Fully interactive CLI menu
JSON-based persistence
Category management
Expense tracking with notes
Date filters (Today, Week, Month, Custom Range)
Layered architecture for clean maintainability


