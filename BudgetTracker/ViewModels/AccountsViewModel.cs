using System.Collections.ObjectModel;
using BudgetTracker.Data;
using BudgetTracker.Models;

namespace BudgetTracker.ViewModels;

public partial class AccountsViewModel : ViewModelBase
{
    public ObservableCollection<AccountBalanceRow> AccountBalances { get; } = [];

    public AccountsViewModel() => _ = LoadAsync();

    public async Task LoadAsync()
    {
        await using var db = new BudgetDbContext();
        var rows = await db.Accounts
            .Select(a => new AccountBalanceRow
            {
                AccountName = a.Name,
                Income = a.Transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expense = a.Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            })
            .ToListAsync();

        AccountBalances.Clear();
        foreach (var row in rows)
        {
            row.Balance = row.Income - row.Expense;
            AccountBalances.Add(row);
        }
    }
}

public class AccountBalanceRow
{
    public string AccountName { get; set; } = string.Empty;
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Balance { get; set; }
}
