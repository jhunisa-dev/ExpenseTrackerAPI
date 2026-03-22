using System;

namespace ExpenseTrackerAPI.DTOs
{
    public class TransactionFilterParams
    {
        // Date range filtering
        // GET /api/transactions?startDate=2026-01-01&endDate=2026-01-31
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Type filtering
        // GET /api/transactions?type=EXPENSE
        public string? Type { get; set; }

        // Category filtering
        // GET /api/transactions?categoryId=123
        public int? CategoryId { get; set; }

        // Pagination — good practice for large datasets
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}