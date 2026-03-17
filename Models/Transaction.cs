namespace ExpenseTrackerAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key to User
        public int CategoryId { get; set; } // Foreign Key to Category
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }

        // Navigation properties (tells EF Core how tables relate)
        public User? User { get; set; }
        public Category? Category { get; set; }
    }
}