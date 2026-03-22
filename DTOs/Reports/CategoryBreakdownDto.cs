namespace ExpenseTrackerAPI.DTOs.Reports
{
    public class CategoryBreakdownDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "INCOME" or "EXPENSE"
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; } // % of total income/expense
    }
}