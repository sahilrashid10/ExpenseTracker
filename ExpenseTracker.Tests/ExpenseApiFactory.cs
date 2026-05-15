using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ExpenseTracker.Tests;

public sealed class ExpenseApiFactory : WebApplicationFactory<Program>
{
    public string StorePath { get; private set; } = string.Empty;

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        StorePath = Path.Combine(Path.GetTempPath(), $"expenses-{Guid.NewGuid():N}.json");
        builder.UseSetting("ExpenseStorePath", StorePath);
        builder.UseSetting("EnableSeedData", "false");
    }
}