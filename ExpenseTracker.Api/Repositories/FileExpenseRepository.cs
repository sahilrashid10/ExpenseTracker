using System.Text.Json;
using System.Text.Json.Serialization;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Repositories;

public class FileExpenseRepository : IExpenseRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _serializerOptions;

    public FileExpenseRepository(string filePath)
    {
        _filePath = filePath;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var directoryPath = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    public async Task<List<Expense>> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            await SaveAsync(new List<Expense>(), cancellationToken);
            return [];
        }

        try
        {
            await using var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var expenses = await JsonSerializer.DeserializeAsync<List<Expense>>(stream, _serializerOptions, cancellationToken);
            return expenses ?? [];
        }
        catch (JsonException)
        {
            await SaveAsync(new List<Expense>(), cancellationToken);
            return [];
        }
    }

    public async Task SaveAsync(List<Expense> expenses, CancellationToken cancellationToken = default)
    {
        var directoryPath = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        await using var stream = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, expenses, _serializerOptions, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }
}