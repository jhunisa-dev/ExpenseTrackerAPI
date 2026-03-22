using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs.Reports
{
    public class ReportFilterParams
    {
        // Used by monthly: /api/reports/summary/monthly?year=2026&month=1
        public int? Month { get; set; }

        // Used by yearly: /api/reports/summary/yearly?year=2026
        public int? Year { get; set; }

        // Used by category-breakdown and cashflow for custom date ranges
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Optional type filter for category-breakdown
        // GET /api/reports/category-breakdown?type=EXPENSE
        public string? Type { get; set; }
    }
}