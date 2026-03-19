namespace ExpenseTrackerAPI.DTOs
{
    public class UpdateProfileRequest
    {
        public string? Username { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? PreferredCurrency { get; set; }
    }
}