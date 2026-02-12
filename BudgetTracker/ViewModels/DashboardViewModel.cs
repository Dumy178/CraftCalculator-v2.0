using BudgetTracker.Data;
using BudgetTracker.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace BudgetTracker.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty] private string selectedMonth = $"{DateTime.Today:yyyy-MM}";
    [ObservableProperty] private decimal totalIncome;
    [ObservableProperty] private decimal totalExpense;
    [ObservableProperty] private decimal netBalance;

    public ISeries[] ExpenseByCategorySeries { get; private set; } = [];
    public ISeries[] IncomeVsExpenseSeries { get; private set; } = [];
    public Axis[] XAxes { get; private set; } = [];

    partial void OnSelectedMonthChanged(string value) => _ = LoadAsync();

    public DashboardViewModel() => _ = LoadAsync();

    public async Task LoadAsync()
    {
        await using var db = new BudgetDbContext();
        var monthTx = db.Transactions.Where(t => t.MonthKey == SelectedMonth);

        TotalIncome = await monthTx.Where(t => t.Type == TransactionType.Income).SumAsync(t => t.Amount);
        TotalExpense = await monthTx.Where(t => t.Type == TransactionType.Expense).SumAsync(t => t.Amount);
        NetBalance = TotalIncome - TotalExpense;

        var expenseCategories = await monthTx
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.Category!.Name)
            .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
            .ToListAsync();

        XAxes = [new Axis { Labels = expenseCategories.Select(e => e.Category).ToArray() }];
        ExpenseByCategorySeries =
        [
            new ColumnSeries<decimal>
            {
                Values = expenseCategories.Select(x => x.Total).ToArray(),
                Fill = new SolidColorPaint(SKColors.CornflowerBlue)
            }
        ];

        IncomeVsExpenseSeries =
        [
            new PieSeries<decimal> { Values = [TotalIncome], Name = "Income" },
            new PieSeries<decimal> { Values = [TotalExpense], Name = "Expense" }
        ];

        OnPropertyChanged(nameof(ExpenseByCategorySeries));
        OnPropertyChanged(nameof(IncomeVsExpenseSeries));
        OnPropertyChanged(nameof(XAxes));
    }
}
