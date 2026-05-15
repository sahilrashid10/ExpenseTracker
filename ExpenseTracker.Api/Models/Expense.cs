namespace ExpenseTracker.Api.Models;

public class Expense
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public ExpenseCategory Category { get; set; }

    public DateTime ExpenseDate { get; set; }

    public PaymentMode PaymentMode { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}