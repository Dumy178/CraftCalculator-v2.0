using System.Collections.ObjectModel;
using BudgetTracker.Commands;
using BudgetTracker.Data;
using BudgetTracker.Models;

namespace BudgetTracker.ViewModels;

public partial class CategoriesViewModel : ViewModelBase
{
    public ObservableCollection<Category> Categories { get; } = [];
    public IEnumerable<TransactionType> TransactionTypes { get; } = Enum.GetValues<TransactionType>();

    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private TransactionType type = TransactionType.Expense;

    public RelayCommand AddCategoryCommand { get; }
    public RelayCommand DeleteCategoryCommand { get; }

    public CategoriesViewModel()
    {
        AddCategoryCommand = new RelayCommand(async _ => await AddAsync());
        DeleteCategoryCommand = new RelayCommand(async category => await DeleteAsync(category as Category));
        _ = LoadAsync();
    }

    public async Task LoadAsync()
    {
        await using var db = new BudgetDbContext();
        var data = await db.Categories.OrderBy(c => c.Type).ThenBy(c => c.Name).ToListAsync();
        Categories.Clear();
        foreach (var category in data) Categories.Add(category);
    }

    private async Task AddAsync()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;
        await using var db = new BudgetDbContext();
        db.Categories.Add(new Category { Name = Name.Trim(), Type = Type });
        await db.SaveChangesAsync();
        Name = string.Empty;
        await LoadAsync();
    }

    private async Task DeleteAsync(Category? category)
    {
        if (category is null) return;
        await using var db = new BudgetDbContext();
        var existing = await db.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
        if (existing is null) return;
        db.Categories.Remove(existing);
        await db.SaveChangesAsync();
        await LoadAsync();
    }
}
