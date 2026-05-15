using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.Repositories;

namespace ExpenseTracker.Api.Data;

public static class ExpenseSeeder
{
    public static async Task SeedAsync(IExpenseRepository repository, CancellationToken cancellationToken = default)
    {
        var expenses = await repository.LoadAsync(cancellationToken);

        if (expenses.Count > 0)
        {
            return;
        }

        var seededExpenses = new List<Expense>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Lunch at office",
                Amount = 15.50m,
                Category = ExpenseCategory.Food,
                ExpenseDate = DateTime.UtcNow.Date.AddDays(-2),
                PaymentMode = PaymentMode.Card,
                Notes = "Team lunch",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Metro card recharge",
                Amount = 40m,
                Category = ExpenseCategory.Travel,
                ExpenseDate = DateTime.UtcNow.Date.AddDays(-1),
                PaymentMode = PaymentMode.UPI,
                Notes = "Commute",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Internet bill",
                Amount = 799m,
                Category = ExpenseCategory.Bills,
                ExpenseDate = DateTime.UtcNow.Date,
                PaymentMode = PaymentMode.Cash,
                Notes = "Monthly bill",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Desk organizer",
                Amount = 125m,
                Category = ExpenseCategory.Shopping,
                ExpenseDate = DateTime.UtcNow.Date,
                PaymentMode = PaymentMode.Card,
                Notes = "Workspace item",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Misc office snack",
                Amount = 25m,
                Category = ExpenseCategory.Other,
                ExpenseDate = DateTime.UtcNow.Date,
                PaymentMode = PaymentMode.UPI,
                Notes = "Small purchase",
                CreatedAt = DateTime.UtcNow
            }
        };

        await repository.SaveAsync(seededExpenses, cancellationToken);
    }
}