using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.DTOs;

public class ExpenseQueryOptions
{
    public ExpenseCategory? Category { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }

    public string? Search { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 5;
}