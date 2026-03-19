using System.Security.Claims;

namespace ExpenseTrackerAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Extracts the UserId from the JWT NameIdentifier claim.
        /// </summary>
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("UserId claim not found in token.");

            return int.Parse(claim);
        }
    }
}