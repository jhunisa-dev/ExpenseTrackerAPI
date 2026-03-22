using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs
{
    public class UpdateCategoryRequest
    {
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }

        // Optional on update, but validated if provided
        [RegularExpression("^(INCOME|EXPENSE)$",
            ErrorMessage = "Type must be either 'INCOME' or 'EXPENSE'.")]
        public string? Type { get; set; }

        public string? Icon { get; set; }
        public string? Color { get; set; }
    }
}