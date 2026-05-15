using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Services;

public interface IExpenseService
{
    Task<IReadOnlyList<Expense>> GetAllAsync(ExpenseQueryOptions? query = null, CancellationToken cancellationToken = default);

    Task<Expense> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Expense> CreateAsync(CreateExpenseDto dto, CancellationToken cancellationToken = default);

    Task<Expense> UpdateAsync(Guid id, UpdateExpenseDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ExpenseSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}