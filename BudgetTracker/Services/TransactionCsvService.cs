using System.Globalization;
using BudgetTracker.Models;
using CsvHelper;

namespace BudgetTracker.Services;

public class TransactionCsvService
{
    public void Export(string path, IEnumerable<Transaction> transactions)
    {
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(transactions.Select(t => new
        {
            t.Date,
            t.Type,
            t.Amount,
            t.Description,
            t.MonthKey,
            t.AccountId,
            t.CategoryId
        }));
    }

    public List<TransactionCsvRow> Import(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<TransactionCsvRow>().ToList();
    }
}

public class TransactionCsvRow
{
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string MonthKey { get; set; } = string.Empty;
    public int AccountId { get; set; }
    public int CategoryId { get; set; }
}
