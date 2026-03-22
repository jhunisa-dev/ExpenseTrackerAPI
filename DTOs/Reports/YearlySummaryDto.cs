using System.Collections.Generic;
using System;

namespace ExpenseTrackerAPI.DTOs.Reports
{
    public class YearlySummaryDto
    {
        public int Year { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetSavings => TotalIncome - TotalExpense;
        public decimal SavingsRate =>
            TotalIncome == 0 ? 0 : Math.Round((NetSavings / TotalIncome) * 100, 2);
        public int TransactionCount { get; set; }

        // Monthly breakdown nested inside the yearly view
        public List<MonthlySummaryDto> MonthlyBreakdown { get; set; } = new();
    }
}