using System.Collections.ObjectModel;
using System.Windows.Input;
using BudgetTracker.Commands;

namespace BudgetTracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public DashboardViewModel Dashboard { get; } = new();
    public TransactionsViewModel Transactions { get; } = new();
    public AccountsViewModel Accounts { get; } = new();
    public CategoriesViewModel Categories { get; } = new();
    public SettingsViewModel Settings { get; } = new();

    public ObservableCollection<string> Pages { get; } = ["Dashboard", "Transactions", "Accounts", "Categories", "Settings"];

    [ObservableProperty]
    private string selectedPage = "Dashboard";

    [ObservableProperty]
    private ViewModelBase currentPage;

    public ICommand NavigateCommand { get; }

    public MainViewModel()
    {
        currentPage = Dashboard;
        NavigateCommand = new RelayCommand(page =>
        {
            if (page is not string destination) return;
            SelectedPage = destination;
            CurrentPage = destination switch
            {
                "Transactions" => Transactions,
                "Accounts" => Accounts,
                "Categories" => Categories,
                "Settings" => Settings,
                _ => Dashboard
            };
        });
    }
}
