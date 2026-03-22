namespace ExpenseTrackerAPI.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "INCOME" or "EXPENSE"
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsSystem { get; set; } // true when UserId == null (default category)
    }
}