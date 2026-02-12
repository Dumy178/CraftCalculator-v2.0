using System.Windows;
using BudgetTracker.Data;

namespace BudgetTracker;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await using var db = new BudgetDbContext();
        await db.InitializeAsync();
    }
}
