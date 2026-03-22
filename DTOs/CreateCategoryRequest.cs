using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs
{
    public class CreateCategoryRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        // Must be explicitly set — no guessing whether it's income or expense
        [Required]
        [RegularExpression("^(INCOME|EXPENSE)$",
            ErrorMessage = "Type must be either 'INCOME' or 'EXPENSE'.")]
        public string Type { get; set; } = string.Empty;

        public string Icon { get; set; } = "📦";
        public string Color { get; set; } = "#6B7280";
    }
}