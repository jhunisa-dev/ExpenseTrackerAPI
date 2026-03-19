namespace ExpenseTrackerAPI.Interfaces
{
    public interface IAuthService
    {
        string CreateToken(Models.User user);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}