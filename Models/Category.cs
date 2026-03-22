namespace ExpenseTrackerAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // null = system default, set = user-specific
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "INCOME" or "EXPENSE"
        public string Icon { get; set; } = "📦";
        public string Color { get; set; } = "#6B7280";

        // Navigation properties
        public User? User { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
    }
}