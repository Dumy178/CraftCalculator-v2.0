using BudgetTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Data;

public class BudgetDbContext : DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "budget-tracker.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.MonthKey);
    }

    public async Task InitializeAsync()
    {
        await Database.EnsureCreatedAsync();

        if (!await Accounts.AnyAsync())
        {
            Accounts.AddRange(
                new Account { Name = "MAIB", Currency = "MDL" },
                new Account { Name = "MICB", Currency = "MDL" },
                new Account { Name = "Cash", Currency = "MDL" }
            );
        }

        if (!await Categories.AnyAsync())
        {
            Categories.AddRange(
                new Category { Name = "Salary", Type = TransactionType.Income },
                new Category { Name = "Freelance", Type = TransactionType.Income },
                new Category { Name = "Groceries", Type = TransactionType.Expense },
                new Category { Name = "Utilities", Type = TransactionType.Expense },
                new Category { Name = "Transport", Type = TransactionType.Expense }
            );
        }

        await SaveChangesAsync();

        if (!await Transactions.AnyAsync())
        {
            var account = await Accounts.FirstAsync();
            var salary = await Categories.FirstAsync(c => c.Name == "Salary");
            var groceries = await Categories.FirstAsync(c => c.Name == "Groceries");
            var now = DateTime.Today;
            var monthKey = $"{now:yyyy-MM}";

            Transactions.AddRange(
                new Transaction
                {
                    Date = now.AddDays(-4),
                    Type = TransactionType.Income,
                    AccountId = account.Id,
                    CategoryId = salary.Id,
                    Amount = 22000m,
                    Description = "Monthly salary",
                    MonthKey = monthKey
                },
                new Transaction
                {
                    Date = now.AddDays(-2),
                    Type = TransactionType.Expense,
                    AccountId = account.Id,
                    CategoryId = groceries.Id,
                    Amount = 1450m,
                    Description = "Weekly groceries",
                    MonthKey = monthKey
                }
            );

            await SaveChangesAsync();
        }
    }
}
