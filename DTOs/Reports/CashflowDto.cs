using System.Collections.Generic;

namespace ExpenseTrackerAPI.DTOs.Reports
{
    public class CashflowDto
    {
        // Summary totals for the requested period
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetCashflow => TotalIncome - TotalExpense;
        public string CashflowStatus => // Quick status label
            NetCashflow > 0 ? "SURPLUS" : NetCashflow < 0 ? "DEFICIT" : "BREAK_EVEN";

        // Daily data points — used for drawing cashflow charts on the frontend
        public List<DailyCashflowDto> DailyBreakdown { get; set; } = new();
    }

    public class DailyCashflowDto
    {
        public DateOnly Date { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Net => Income - Expense;
        public decimal RunningBalance { get; set; } // Cumulative net over time
    }
}