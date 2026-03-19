using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Extensions;
using ExpenseTrackerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public UsersController(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // GET /api/users/profile
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = User.GetUserId();

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                return NotFound("User not found.");

            return Ok(MapToDto(user));
        }

        // PUT /api/users/profile
        [HttpPut("profile")]
        public async Task<ActionResult<UserProfileDto>> UpdateProfile(UpdateProfileRequest request)
        {
            var userId = User.GetUserId();

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                return NotFound("User not found.");

            // Update Username
            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                bool usernameTaken = await _context.Users
                    .AnyAsync(u => u.Username == request.Username && u.Id != userId);

                if (usernameTaken)
                    return BadRequest("Username is already taken.");

                user.Username = request.Username;
            }

            // Update Currency preference
            if (!string.IsNullOrWhiteSpace(request.PreferredCurrency))
                user.PreferredCurrency = request.PreferredCurrency.ToUpper();

            // Update Password (requires current password verification) 
            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                    return BadRequest("Current password is required to set a new password.");

                if (!_authService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                    return BadRequest("Current password is incorrect.");

                user.PasswordHash = _authService.HashPassword(request.NewPassword);
            }

            await _context.SaveChangesAsync();

            return Ok(MapToDto(user));
        }

        //  Private mapper (keeps controller clean) 
        private static UserProfileDto MapToDto(Models.User user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PreferredCurrency = user.PreferredCurrency,
            CreatedAt = user.CreatedAt
        };
    }
}