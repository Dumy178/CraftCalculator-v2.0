# Budget Tracker (WPF, .NET 8)

A desktop budgeting app scaffold using:

- C# / .NET 8
- WPF + MVVM
- SQLite + Entity Framework Core
- LiveCharts2 for dashboard charts
- CSV export service
- Light/Dark theme switch

## Features included

- Dashboard with monthly income, expense, and net totals
- Expense by category (bar chart)
- Income vs expense (pie chart)
- Transactions add/delete + month filtering
- Account balance view
- Settings page with dark mode toggle
- SQLite local database with seed data

## Run

```bash
dotnet restore
dotnet run --project BudgetTracker/BudgetTracker.csproj
```

> If running on Linux/macOS CI, WPF build will not be supported; use Windows with .NET 8 SDK.
