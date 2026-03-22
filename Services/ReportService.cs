using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.DTOs.Reports;
using ExpenseTrackerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ExpenseTrackerAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/reports/summary/monthly
        // Returns one row per month. If ?year=2026&month=1 → single month.
        // If only ?year=2026 → all 12 months for that year.
        // If no params → current year all months.
        public async Task<List<MonthlySummaryDto>> GetMonthlySummaryAsync(
            int userId, ReportFilterParams filters)
        {
            var year = filters.Year ?? DateTime.UtcNow.Year;

            var query = _context.Transactions
                .Where(t => t.UserId == userId && t.Date.Year == year);

            // Narrow to a single month when specified
            if (filters.Month.HasValue)
                query = query.Where(t => t.Date.Month == filters.Month.Value);

            // GroupBy Year+Month to get one summary row per month
            var grouped = await query
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalIncome = g.Where(t => t.Type == "INCOME").Sum(t => t.Amount),
                    TotalExpense = g.Where(t => t.Type == "EXPENSE").Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToListAsync();

            return grouped.Select(g => new MonthlySummaryDto
            {
                Year = g.Year,
                Month = g.Month,
                MonthName = new DateTime(g.Year, g.Month, 1)
                                       .ToString("MMMM"), // "January", "February"...
                TotalIncome = g.TotalIncome,
                TotalExpense = g.TotalExpense,
                TransactionCount = g.TransactionCount
            }).ToList();
        }

        // GET /api/reports/summary/yearly
        // Returns one row per year with a nested monthly breakdown.
        // If ?year=2026 → only that year. If no params → all years.
        public async Task<List<YearlySummaryDto>> GetYearlySummaryAsync(
            int userId, ReportFilterParams filters)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId);

            if (filters.Year.HasValue)
                query = query.Where(t => t.Date.Year == filters.Year.Value);

            // GroupBy Year for the top-level summary
            var yearlyGrouped = await query
                .GroupBy(t => t.Date.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    TotalIncome = g.Where(t => t.Type == "INCOME").Sum(t => t.Amount),
                    TotalExpense = g.Where(t => t.Type == "EXPENSE").Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(g => g.Year)
                .ToListAsync();

            // GroupBy Year+Month for the nested monthly breakdown
            var monthlyGrouped = await query
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalIncome = g.Where(t => t.Type == "INCOME").Sum(t => t.Amount),
                    TotalExpense = g.Where(t => t.Type == "EXPENSE").Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .ToListAsync();

            // Assemble yearly DTOs with their monthly children
            return yearlyGrouped.Select(y => new YearlySummaryDto
            {
                Year = y.Year,
                TotalIncome = y.TotalIncome,
                TotalExpense = y.TotalExpense,
                TransactionCount = y.TransactionCount,
                MonthlyBreakdown = monthlyGrouped
                    .Where(m => m.Year == y.Year)
                    .OrderBy(m => m.Month)
                    .Select(m => new MonthlySummaryDto
                    {
                        Year = m.Year,
                        Month = m.Month,
                        MonthName = new DateTime(m.Year, m.Month, 1).ToString("MMMM"),
                        TotalIncome = m.TotalIncome,
                        TotalExpense = m.TotalExpense,
                        TransactionCount = m.TransactionCount
                    }).ToList()
            }).ToList();
        }

        // GET /api/reports/category-breakdown
        // GET /api/reports/category-breakdown?type=EXPENSE
        // GET /api/reports/category-breakdown?startDate=2026-01-01&endDate=2026-01-31
        public async Task<List<CategoryBreakdownDto>> GetCategoryBreakdownAsync(
            int userId, ReportFilterParams filters)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId);

            // Apply optional filters
            if (filters.StartDate.HasValue)
                query = query.Where(t => t.Date >= filters.StartDate.Value);

            if (filters.EndDate.HasValue)
                query = query.Where(t => t.Date <= filters.EndDate.Value.AddDays(1).AddTicks(-1));

            if (!string.IsNullOrWhiteSpace(filters.Type))
                query = query.Where(t => t.Type == filters.Type.ToUpper());

            // GroupBy CategoryId and sum amounts per category
            var grouped = await query
                .GroupBy(t => new
                {
                    t.CategoryId,
                    t.Category!.Name,
                    t.Category.Icon,
                    t.Category.Color,
                    t.Type
                })
                .Select(g => new
                {
                    g.Key.CategoryId,
                    g.Key.Name,
                    g.Key.Icon,
                    g.Key.Color,
                    g.Key.Type,
                    TotalAmount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(g => g.TotalAmount)
                .ToListAsync();

            // Calculate percentage share per type separately
            var totalExpense = grouped
                .Where(g => g.Type == "EXPENSE")
                .Sum(g => g.TotalAmount);

            var totalIncome = grouped
                .Where(g => g.Type == "INCOME")
                .Sum(g => g.TotalAmount);

            return grouped.Select(g => new CategoryBreakdownDto
            {
                CategoryId = g.CategoryId,
                CategoryName = g.Name,
                CategoryIcon = g.Icon,
                CategoryColor = g.Color,
                Type = g.Type,
                TotalAmount = g.TotalAmount,
                TransactionCount = g.TransactionCount,

                // Each category's share of its type's total (EXPENSE % of total expenses)
                Percentage = g.Type == "EXPENSE" && totalExpense > 0
                    ? Math.Round((g.TotalAmount / totalExpense) * 100, 2)
                    : g.Type == "INCOME" && totalIncome > 0
                    ? Math.Round((g.TotalAmount / totalIncome) * 100, 2)
                    : 0
            }).ToList();
        }

        // GET /api/reports/cashflow
        // GET /api/reports/cashflow?startDate=2026-01-01&endDate=2026-01-31
        // Defaults to current month when no date range given.
        public async Task<CashflowDto> GetCashflowAsync(int userId, ReportFilterParams filters)
        {
            // Default to current month if no range provided
            var startDate = filters.StartDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var endDate = filters.EndDate ?? startDate.AddMonths(1).AddTicks(-1);

            var transactions = await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.Date >= startDate &&
                    t.Date <= endDate)
                .OrderBy(t => t.Date)
                .ToListAsync();

            var totalIncome = transactions.Where(t => t.Type == "INCOME").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.Type == "EXPENSE").Sum(t => t.Amount);

            // ✅ GroupBy day to build the daily breakdown for charting
            var dailyGroups = transactions
                .GroupBy(t => DateOnly.FromDateTime(t.Date))
                .OrderBy(g => g.Key)
                .ToList();

            var dailyBreakdown = new List<DailyCashflowDto>();
            decimal runningBalance = 0;

            foreach (var day in dailyGroups)
            {
                var income = day.Where(t => t.Type == "INCOME").Sum(t => t.Amount);
                var expense = day.Where(t => t.Type == "EXPENSE").Sum(t => t.Amount);
                runningBalance += income - expense;  // ✅ Cumulative net balance over time

                dailyBreakdown.Add(new DailyCashflowDto
                {
                    Date = day.Key,
                    Income = income,
                    Expense = expense,
                    RunningBalance = runningBalance
                });
            }

            return new CashflowDto
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                DailyBreakdown = dailyBreakdown
            };
        }
    }
}