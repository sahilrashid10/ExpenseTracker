namespace ExpenseTracker.Api.ViewModels;

public class ExpenseSummaryViewModel
{
    public decimal TotalExpense { get; set; }

    public int ExpenseCount { get; set; }

    public Dictionary<string, decimal> ByCategory { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, decimal> ByMonth { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}