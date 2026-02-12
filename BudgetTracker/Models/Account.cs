namespace BudgetTracker.Models;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "MDL";
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
