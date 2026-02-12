using System.Collections.ObjectModel;
using BudgetTracker.Commands;
using BudgetTracker.Data;
using BudgetTracker.Models;
using BudgetTracker.Services;

namespace BudgetTracker.ViewModels;

public partial class TransactionsViewModel : ViewModelBase
{
    private readonly TransactionCsvService _csvService = new();
    private List<Category> _allCategories = [];

    public ObservableCollection<Transaction> Transactions { get; } = [];
    public ObservableCollection<Account> Accounts { get; } = [];
    public ObservableCollection<Category> Categories { get; } = [];
    public IEnumerable<TransactionType> TransactionTypes { get; } = Enum.GetValues<TransactionType>();

    [ObservableProperty] private DateTime date = DateTime.Today;
    [ObservableProperty] private TransactionType type = TransactionType.Expense;
    [ObservableProperty] private Account? selectedAccount;
    [ObservableProperty] private Category? selectedCategory;
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string selectedMonth = $"{DateTime.Today:yyyy-MM}";

    public RelayCommand AddTransactionCommand { get; }
    public RelayCommand DeleteTransactionCommand { get; }
    public RelayCommand ExportCsvCommand { get; }
    public RelayCommand ImportCsvCommand { get; }

    public TransactionsViewModel()
    {
        AddTransactionCommand = new RelayCommand(async _ => await AddAsync());
        DeleteTransactionCommand = new RelayCommand(async tx => await DeleteAsync(tx as Transaction));
        ExportCsvCommand = new RelayCommand(_ => _csvService.Export("transactions-export.csv", Transactions));
        ImportCsvCommand = new RelayCommand(async _ => await ImportCsvAsync());
        _ = LoadAsync();
    }

    public async Task LoadAsync()
    {
        await using var db = new BudgetDbContext();

        var accounts = await db.Accounts.OrderBy(a => a.Name).ToListAsync();
        var categories = await db.Categories.OrderBy(c => c.Name).ToListAsync();
        _allCategories = categories;
        var transactions = await db.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Where(t => t.MonthKey == SelectedMonth)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        Accounts.Clear();
        Categories.Clear();
        Transactions.Clear();

        foreach (var a in accounts) Accounts.Add(a);
        foreach (var c in _allCategories.Where(c => c.Type == Type)) Categories.Add(c);
        foreach (var t in transactions) Transactions.Add(t);

        SelectedAccount ??= Accounts.FirstOrDefault();
        SelectedCategory ??= Categories.FirstOrDefault();
    }

    partial void OnTypeChanged(TransactionType value)
    {
        var filtered = _allCategories.Where(c => c.Type == value).ToList();
        Categories.Clear();
        foreach (var c in filtered) Categories.Add(c);
        SelectedCategory = Categories.FirstOrDefault();
    }

    partial void OnSelectedMonthChanged(string value) => _ = LoadAsync();

    private async Task AddAsync()
    {
        if (SelectedAccount is null || SelectedCategory is null || Amount <= 0) return;

        await using var db = new BudgetDbContext();
        var tx = new Transaction
        {
            Date = Date,
            Type = Type,
            AccountId = SelectedAccount.Id,
            CategoryId = SelectedCategory.Id,
            Amount = Amount,
            Description = Description,
            MonthKey = SelectedMonth
        };

        db.Transactions.Add(tx);
        await db.SaveChangesAsync();
        await LoadAsync();
    }

    private async Task DeleteAsync(Transaction? transaction)
    {
        if (transaction is null) return;

        await using var db = new BudgetDbContext();
        var existing = await db.Transactions.FirstOrDefaultAsync(t => t.Id == transaction.Id);
        if (existing is null) return;

        db.Transactions.Remove(existing);
        await db.SaveChangesAsync();
        await LoadAsync();
    }
    private async Task ImportCsvAsync()
    {
        const string path = "transactions-import.csv";
        if (!File.Exists(path)) return;

        var rows = _csvService.Import(path);
        if (rows.Count == 0) return;

        await using var db = new BudgetDbContext();
        foreach (var row in rows)
        {
            var type = Enum.TryParse<TransactionType>(row.Type, out var parsedType) ? parsedType : TransactionType.Expense;
            db.Transactions.Add(new Transaction
            {
                Date = row.Date,
                Type = type,
                Amount = row.Amount,
                Description = row.Description,
                MonthKey = row.MonthKey,
                AccountId = row.AccountId,
                CategoryId = row.CategoryId
            });
        }

        await db.SaveChangesAsync();
        await LoadAsync();
    }

}
