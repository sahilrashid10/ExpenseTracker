using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.ViewModels;

public class ExpenseDashboardViewModel
{
    public List<Expense> Expenses { get; set; } = [];

    public Expense? SelectedExpense { get; set; }

    public ExpenseSummaryViewModel Summary { get; set; } = new();

    public CreateExpenseFormModel CreateExpense { get; set; } = new();

    public ExpenseQueryOptions Query { get; set; } = new();

    public bool HasPreviousPage { get; set; }

    public bool HasNextPage { get; set; }

    public decimal ThisMonthSpend { get; set; }

    public decimal AverageDailySpend { get; set; }

    public string TopCategory { get; set; } = "-";

    public decimal TopCategoryAmount { get; set; }

    public List<CategoryChartItem> CategoryChart { get; set; } = [];

    public List<MonthlyTrendItem> MonthlyTrend { get; set; } = [];
}

public class CategoryChartItem
{
    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal SharePercent { get; set; }
}

public class MonthlyTrendItem
{
    public string Label { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal HeightPercent { get; set; }
}