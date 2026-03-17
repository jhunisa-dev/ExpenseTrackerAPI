using System.Transactions;

namespace ExpenseTrackerAPI.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property: A user can have many transactions
        public List<Transaction> Transactions { get; set; } = new();
    }
}