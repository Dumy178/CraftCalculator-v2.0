namespace BudgetTracker.Models;

public class Transaction
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string MonthKey { get; set; } = string.Empty;

    public int AccountId { get; set; }
    public Account? Account { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
