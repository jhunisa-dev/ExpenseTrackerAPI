using System;

namespace ExpenseTrackerAPI.DTOs.Reports
{
    public class MonthlySummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetSavings => TotalIncome - TotalExpense; // Computed, not stored
        public decimal SavingsRate => // % of income saved
            TotalIncome == 0 ? 0 : Math.Round((NetSavings / TotalIncome) * 100, 2);
        public int TransactionCount { get; set; }
    }
}