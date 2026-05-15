using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Repositories;

public interface IExpenseRepository
{
    Task<List<Expense>> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(List<Expense> expenses, CancellationToken cancellationToken = default);
}