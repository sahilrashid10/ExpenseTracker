using ExpenseTracker.Api.Middleware;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Repositories;
using ExpenseTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();

var expenseStorePath = builder.Configuration["ExpenseStorePath"];
var resolvedExpenseStorePath = string.IsNullOrWhiteSpace(expenseStorePath)
    ? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "expenses.json")
    : Path.IsPathRooted(expenseStorePath)
        ? expenseStorePath
        : Path.Combine(builder.Environment.ContentRootPath, expenseStorePath);

builder.Services.AddSingleton<IExpenseRepository>(_ => new FileExpenseRepository(resolvedExpenseStorePath));
builder.Services.AddScoped<IExpenseService, ExpenseService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseStaticFiles();

app.MapDefaultControllerRoute();
app.MapControllers();

var enableSeedData = builder.Configuration.GetValue("EnableSeedData", true);
if (enableSeedData)
{
    using var scope = app.Services.CreateScope();
    var repository = scope.ServiceProvider.GetRequiredService<IExpenseRepository>();
    await ExpenseSeeder.SeedAsync(repository);
}

app.Run();

public partial class Program { }
