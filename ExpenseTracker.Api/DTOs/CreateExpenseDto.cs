using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.DTOs;

public class CreateExpenseDto
{
    [Required]
    [MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal Amount { get; set; }

    [Required]
    public ExpenseCategory Category { get; set; }

    [Required]
    public DateTime ExpenseDate { get; set; }

    [Required]
    public PaymentMode PaymentMode { get; set; }

    public string? Notes { get; set; }
}