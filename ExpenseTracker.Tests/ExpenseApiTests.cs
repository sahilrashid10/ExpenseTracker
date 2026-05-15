using System.Net;
using System.Net.Http.Json;
using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Tests;

public class ExpenseApiTests
{
    [Fact]
    public async Task AddValidExpense_ReturnsCreated()
    {
        using var factory = new ExpenseApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/expenses", new CreateExpenseDto
        {
            Title = "Lunch",
            Amount = 12.50m,
            Category = ExpenseCategory.Food,
            ExpenseDate = new DateTime(2026, 04, 11, 10, 0, 0, DateTimeKind.Utc),
            PaymentMode = PaymentMode.Card,
            Notes = "Office cafeteria"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdExpense = await response.Content.ReadFromJsonAsync<Expense>();
        Assert.NotNull(createdExpense);
        Assert.NotEqual(Guid.Empty, createdExpense!.Id);
        Assert.True(createdExpense.CreatedAt != default);
    }

    [Fact]
    public async Task AddInvalidExpense_ReturnsBadRequest()
    {
        using var factory = new ExpenseApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/expenses", new CreateExpenseDto
        {
            Title = "Invalid",
            Amount = 0,
            Category = ExpenseCategory.Other,
            ExpenseDate = DateTime.UtcNow,
            PaymentMode = PaymentMode.Cash
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAllExpenses_ReturnsOkAndEmptyListInitially()
    {
        using var factory = new ExpenseApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/expenses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var expenses = await response.Content.ReadFromJsonAsync<List<Expense>>();
        Assert.NotNull(expenses);
        Assert.Empty(expenses!);
    }

    [Fact]
    public async Task GetNonExistingExpense_ReturnsNotFound()
    {
        using var factory = new ExpenseApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/expenses/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteExpense_ReturnsNoContent()
    {
        using var factory = new ExpenseApiFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/expenses", new CreateExpenseDto
        {
            Title = "Taxi",
            Amount = 30m,
            Category = ExpenseCategory.Travel,
            ExpenseDate = DateTime.UtcNow,
            PaymentMode = PaymentMode.UPI
        });

        var createdExpense = await createResponse.Content.ReadFromJsonAsync<Expense>();
        Assert.NotNull(createdExpense);

        var deleteResponse = await client.DeleteAsync($"/expenses/{createdExpense!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task FilterByCategory_ReturnsMatchingExpenses()
    {
        using var factory = new ExpenseApiFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/expenses", new CreateExpenseDto
        {
            Title = "Groceries",
            Amount = 40m,
            Category = ExpenseCategory.Food,
            ExpenseDate = DateTime.UtcNow,
            PaymentMode = PaymentMode.Card
        });

        await client.PostAsJsonAsync("/expenses", new CreateExpenseDto
        {
            Title = "Fuel",
            Amount = 60m,
            Category = ExpenseCategory.Travel,
            ExpenseDate = DateTime.UtcNow,
            PaymentMode = PaymentMode.Cash
        });

        var response = await client.GetAsync("/expenses?category=Food");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var expenses = await response.Content.ReadFromJsonAsync<List<Expense>>();
        Assert.NotNull(expenses);
        Assert.All(expenses!, expense => Assert.Equal(ExpenseCategory.Food, expense.Category));
    }

    [Fact]
    public async Task Summary_ReturnsTotalExpense()
    {
        using var factory = new ExpenseApiFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/expenses", new CreateExpenseDto
        {
            Title = "Books",
            Amount = 20m,
            Category = ExpenseCategory.Shopping,
            ExpenseDate = new DateTime(2026, 04, 01, 0, 0, 0, DateTimeKind.Utc),
            PaymentMode = PaymentMode.Card
        });

        await client.PostAsJsonAsync("/expenses", new CreateExpenseDto
        {
            Title = "Tea",
            Amount = 5m,
            Category = ExpenseCategory.Food,
            ExpenseDate = new DateTime(2026, 04, 02, 0, 0, 0, DateTimeKind.Utc),
            PaymentMode = PaymentMode.Cash
        });

        var response = await client.GetAsync("/expenses/summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var summary = await response.Content.ReadFromJsonAsync<ExpenseSummaryDto>();
        Assert.NotNull(summary);
        Assert.Equal(25m, summary!.TotalExpense);
    }
}