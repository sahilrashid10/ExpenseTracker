namespace ExpenseTracker.Api.Exceptions;

public class ExpenseNotFoundException : Exception
{
    public ExpenseNotFoundException(Guid expenseId)
        : base($"Expense '{expenseId}' was not found.")
    {
        ExpenseId = expenseId;
    }

    public Guid ExpenseId { get; }
}