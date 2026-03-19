namespace ExpenseTrackerAPI.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PreferredCurrency { get; set; } = "USD";
        public DateTime CreatedAt { get; set; }
    }
}