namespace ExpenseTrackerAPI.Models
{
    public class Category
    {
        public int Id { get; set; } // Primary Key
        public int? UserId { get; set; } // Nullable! If null, it's a default system category
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "INCOME" or "EXPENSE"
    }
}