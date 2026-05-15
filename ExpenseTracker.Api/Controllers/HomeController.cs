using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Api.Services;
using ExpenseTracker.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

public class HomeController : Controller
{
    private readonly IExpenseService _expenseService;

    public HomeController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index([FromQuery] ExpenseQueryOptions query, CancellationToken cancellationToken)
    {
        query.Page = query.Page <= 0 ? 1 : query.Page;
        query.PageSize = query.PageSize <= 0 ? 5 : query.PageSize;

        var expenses = (await _expenseService.GetAllAsync(query, cancellationToken)).ToList();
        var summary = await _expenseService.GetSummaryAsync(cancellationToken);
        var nextPageQuery = new ExpenseQueryOptions
        {
            Category = query.Category,
            FromDate = query.FromDate,
            ToDate = query.ToDate,
            SortBy = query.SortBy,
            Descending = query.Descending,
            Search = query.Search,
            Page = query.Page + 1,
            PageSize = query.PageSize
        };

        var nextPageExpenses = await _expenseService.GetAllAsync(nextPageQuery, cancellationToken);

        var totalExpense = expenses.Sum(expense => expense.Amount);
        var thisMonthSpend = expenses
            .Where(expense => expense.ExpenseDate.Year == DateTime.Today.Year && expense.ExpenseDate.Month == DateTime.Today.Month)
            .Sum(expense => expense.Amount);
        var averageDailySpend = expenses.Count == 0
            ? 0
            : expenses.Sum(expense => expense.Amount) / Math.Max(1, expenses.Select(expense => expense.ExpenseDate.Date).Distinct().Count());
        var categoryTotals = expenses
            .GroupBy(expense => expense.Category)
            .Select(group => new { Category = group.Key.ToString(), Amount = group.Sum(expense => expense.Amount) })
            .OrderByDescending(item => item.Amount)
            .ToList();
        var topCategory = categoryTotals.FirstOrDefault();
        var maxCategoryAmount = categoryTotals.Select(item => item.Amount).DefaultIfEmpty(0).Max();
        var monthlyTotals = expenses
            .GroupBy(expense => expense.ExpenseDate.ToString("yyyy-MM"))
            .Select(group => new { Month = group.Key, Amount = group.Sum(expense => expense.Amount) })
            .OrderBy(item => item.Month)
            .ToList();
        var maxMonthAmount = monthlyTotals.Select(item => item.Amount).DefaultIfEmpty(0).Max();

        var model = new ExpenseDashboardViewModel
        {
            Expenses = expenses,
            Query = query,
            HasPreviousPage = query.Page > 1,
            HasNextPage = nextPageExpenses.Count > 0,
            ThisMonthSpend = thisMonthSpend,
            AverageDailySpend = averageDailySpend,
            TopCategory = topCategory?.Category ?? "-",
            TopCategoryAmount = topCategory?.Amount ?? 0,
            CategoryChart = categoryTotals.Select(item => new CategoryChartItem
            {
                Name = item.Category,
                Amount = item.Amount,
                SharePercent = maxCategoryAmount == 0 ? 0 : item.Amount / maxCategoryAmount * 100
            }).ToList(),
            MonthlyTrend = monthlyTotals.Select(item => new MonthlyTrendItem
            {
                Label = item.Month,
                Amount = item.Amount,
                HeightPercent = maxMonthAmount == 0 ? 0 : item.Amount / maxMonthAmount * 100
            }).ToList(),
            Summary = new ExpenseSummaryViewModel
            {
                TotalExpense = summary.TotalExpense,
                ExpenseCount = summary.ExpenseCount,
                ByCategory = summary.ByCategory,
                ByMonth = summary.ByMonth
            },
            CreateExpense = new CreateExpenseFormModel
            {
                ExpenseDate = DateTime.Today
            }
        };

        return View(model);
    }

    [HttpGet("expenses/create")]
    public IActionResult Create()
    {
        return View(new CreateExpenseFormModel
        {
            ExpenseDate = DateTime.Today
        });
    }

    [HttpPost("expenses/dashboard-create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFromDashboard([Bind(Prefix = "CreateExpense")] CreateExpenseFormModel model, ExpenseQueryOptions query, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return await Index(query, cancellationToken);
        }

        await _expenseService.CreateAsync(new CreateExpenseDto
        {
            Title = model.Title,
            Amount = model.Amount,
            Category = model.Category,
            ExpenseDate = model.ExpenseDate,
            PaymentMode = model.PaymentMode,
            Notes = model.Notes
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("expenses/edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromQuery] ExpenseQueryOptions query, CancellationToken cancellationToken)
    {
        var expense = await _expenseService.GetByIdAsync(id, cancellationToken);
        ViewData["ReturnQuery"] = query;

        var model = new EditExpenseFormModel
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            Category = expense.Category,
            ExpenseDate = expense.ExpenseDate,
            PaymentMode = expense.PaymentMode,
            Notes = expense.Notes
        };

        return View(model);
    }

    [HttpPost("expenses/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateExpenseFormModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _expenseService.CreateAsync(new CreateExpenseDto
        {
            Title = model.Title,
            Amount = model.Amount,
            Category = model.Category,
            ExpenseDate = model.ExpenseDate,
            PaymentMode = model.PaymentMode,
            Notes = model.Notes
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("expenses/update/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Guid id, EditExpenseFormModel model, [FromQuery] ExpenseQueryOptions query, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnQuery"] = query;
            return View("Edit", model);
        }

        await _expenseService.UpdateAsync(id, new UpdateExpenseDto
        {
            Title = model.Title,
            Amount = model.Amount,
            Category = model.Category,
            ExpenseDate = model.ExpenseDate,
            PaymentMode = model.PaymentMode,
            Notes = model.Notes
        }, cancellationToken);

        return RedirectToAction(nameof(Index), new
        {
            query.Search,
            query.Category,
            query.FromDate,
            query.ToDate,
            query.SortBy,
            query.Descending,
            query.Page,
            query.PageSize
        });
    }

    [HttpPost("expenses/delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] ExpenseQueryOptions query, CancellationToken cancellationToken)
    {
        await _expenseService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index), new
        {
            query.Search,
            query.Category,
            query.FromDate,
            query.ToDate,
            query.SortBy,
            query.Descending,
            query.Page,
            query.PageSize
        });
    }
}