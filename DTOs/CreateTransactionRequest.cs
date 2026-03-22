using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs
{
    public class CreateTransactionRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression("^(INCOME|EXPENSE)$",
            ErrorMessage = "Type must be either 'INCOME' or 'EXPENSE'.")]
        public string Type { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public string Currency { get; set; } = "USD";

        [StringLength(255)]
        public string Note { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}