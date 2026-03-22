namespace ExpenseTrackerAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key to User
        public int CategoryId { get; set; } // Foreign Key to Category

        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;        // "INCOME" or "EXPENSE"
        public string Currency { get; set; } = "USD";           // Multi-currency support
        public string Note { get; set; } = string.Empty;        // Optional description
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User? User { get; set; }
        public Category? Category { get; set; }
    }
}