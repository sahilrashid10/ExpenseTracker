using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Api.Exceptions;
using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.Repositories;

namespace ExpenseTracker.Api.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repository;

    public ExpenseService(IExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Expense>> GetAllAsync(ExpenseQueryOptions? query = null, CancellationToken cancellationToken = default)
    {
        var expenses = await _repository.LoadAsync(cancellationToken);
        IEnumerable<Expense> filteredExpenses = expenses;

        if (query?.Category is not null)
        {
            filteredExpenses = filteredExpenses.Where(expense => expense.Category == query.Category);
        }

        if (query?.FromDate is not null)
        {
            filteredExpenses = filteredExpenses.Where(expense => expense.ExpenseDate.Date >= query.FromDate.Value.Date);
        }

        if (query?.ToDate is not null)
        {
            filteredExpenses = filteredExpenses.Where(expense => expense.ExpenseDate.Date <= query.ToDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(query?.Search))
        {
            filteredExpenses = filteredExpenses.Where(expense =>
                expense.Title.Contains(query.Search, StringComparison.OrdinalIgnoreCase));
        }

        filteredExpenses = ApplySorting(filteredExpenses, query);
        filteredExpenses = ApplyPagination(filteredExpenses, query);

        return filteredExpenses.ToList();
    }

    public async Task<Expense> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expenses = await _repository.LoadAsync(cancellationToken);
        return expenses.FirstOrDefault(expense => expense.Id == id)
            ?? throw new ExpenseNotFoundException(id);
    }

    public async Task<Expense> CreateAsync(CreateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        ValidateExpenseRequest(dto.Title, dto.Amount);

        var expenses = await _repository.LoadAsync(cancellationToken);
        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Amount = dto.Amount,
            Category = dto.Category,
            ExpenseDate = dto.ExpenseDate,
            PaymentMode = dto.PaymentMode,
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        expenses.Add(expense);
        await _repository.SaveAsync(expenses, cancellationToken);
        return expense;
    }

    public async Task<Expense> UpdateAsync(Guid id, UpdateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        ValidateExpenseRequest(dto.Title, dto.Amount);

        var expenses = await _repository.LoadAsync(cancellationToken);
        var expense = expenses.FirstOrDefault(item => item.Id == id)
            ?? throw new ExpenseNotFoundException(id);

        expense.Title = dto.Title.Trim();
        expense.Amount = dto.Amount;
        expense.Category = dto.Category;
        expense.ExpenseDate = dto.ExpenseDate;
        expense.PaymentMode = dto.PaymentMode;
        expense.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();

        await _repository.SaveAsync(expenses, cancellationToken);
        return expense;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expenses = await _repository.LoadAsync(cancellationToken);
        var expense = expenses.FirstOrDefault(item => item.Id == id)
            ?? throw new ExpenseNotFoundException(id);

        expenses.Remove(expense);
        await _repository.SaveAsync(expenses, cancellationToken);
    }

    public async Task<ExpenseSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var expenses = await _repository.LoadAsync(cancellationToken);

        return new ExpenseSummaryDto
        {
            TotalExpense = expenses.Sum(expense => expense.Amount),
            ExpenseCount = expenses.Count,
            ByCategory = expenses
                .GroupBy(expense => expense.Category.ToString())
                .ToDictionary(group => group.Key, group => group.Sum(expense => expense.Amount), StringComparer.OrdinalIgnoreCase),
            ByMonth = expenses
                .GroupBy(expense => expense.ExpenseDate.ToString("yyyy-MM"))
                .ToDictionary(group => group.Key, group => group.Sum(expense => expense.Amount), StringComparer.OrdinalIgnoreCase)
        };
    }

    private static IEnumerable<Expense> ApplySorting(IEnumerable<Expense> expenses, ExpenseQueryOptions? query)
    {
        var sortBy = query?.SortBy?.Trim().ToLowerInvariant();
        return sortBy switch
        {
            "amount" when query?.Descending == true => expenses.OrderByDescending(expense => expense.Amount),
            "amount" => expenses.OrderBy(expense => expense.Amount),
            "date" when query?.Descending == false => expenses.OrderBy(expense => expense.ExpenseDate),
            "date" => expenses.OrderByDescending(expense => expense.ExpenseDate),
            _ when query?.Descending == true => expenses.OrderByDescending(expense => expense.ExpenseDate),
            _ => expenses.OrderBy(expense => expense.ExpenseDate)
        };
    }

    private static IEnumerable<Expense> ApplyPagination(IEnumerable<Expense> expenses, ExpenseQueryOptions? query)
    {
        var page = query?.Page > 0 ? query.Page : 1;
        var pageSize = query?.PageSize is > 0 and <= 100 ? query.PageSize : 5;

        return expenses.Skip((page - 1) * pageSize).Take(pageSize);
    }

    private static void ValidateExpenseRequest(string? title, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.");
        }
    }
}