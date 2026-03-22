using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs
{
    public class UpdateTransactionRequest
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal? Amount { get; set; }

        [RegularExpression("^(INCOME|EXPENSE)$",
            ErrorMessage = "Type must be either 'INCOME' or 'EXPENSE'.")]
        public string? Type { get; set; }

        public int? CategoryId { get; set; }
        public string? Currency { get; set; }

        [StringLength(255)]
        public string? Note { get; set; }

        public DateTime? Date { get; set; }
    }
}