using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.ViewModels;

public class CreateExpenseFormModel
{
    [Required]
    [MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal Amount { get; set; }

    [Required]
    public ExpenseCategory Category { get; set; } = ExpenseCategory.Other;

    [Required]
    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    [Required]
    public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;

    public string? Notes { get; set; }
}