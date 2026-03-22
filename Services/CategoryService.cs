using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Interfaces;
using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ExpenseTrackerAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync(int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == null || c.UserId == userId)
                .OrderBy(c => c.UserId == null ? 0 : 1) // System defaults first
                .ThenBy(c => c.Type)                    // Then grouped by EXPENSE/INCOME
                .ThenBy(c => c.Name)
                .Select(c => MapToDto(c))
                .ToListAsync();
        }

        public async Task<CategoryDto> CreateCategoryAsync(int userId, CreateCategoryRequest request)
        {
            // Normalize to uppercase defensively even though validation enforces it
            var type = request.Type.ToUpper();

            bool exists = await _context.Categories.AnyAsync(c =>
                c.Name.ToLower() == request.Name.ToLower() &&
                c.Type == type && // Scope duplicate check to same Type
                (c.UserId == null || c.UserId == userId));

            if (exists)
                throw new InvalidOperationException(
                    $"A {type} category named '{request.Name}' already exists.");

            var category = new Category
            {
                Name = request.Name,
                Type = type,                                    
                Icon = request.Icon,
                Color = request.Color,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int userId, int categoryId, UpdateCategoryRequest request)
        {
            var category = await GetUserOwnedCategoryAsync(userId, categoryId);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var typeToCheck = request.Type?.ToUpper() ?? category.Type;

                bool nameConflict = await _context.Categories.AnyAsync(c =>
                    c.Name.ToLower() == request.Name.ToLower() &&
                    c.Type == typeToCheck && // Scope conflict check to Type
                    (c.UserId == null || c.UserId == userId) &&
                    c.Id != categoryId);

                if (nameConflict)
                    throw new InvalidOperationException(
                        $"A {typeToCheck} category named '{request.Name}' already exists.");

                category.Name = request.Name;
            }

            // Allow changing Type (e.g., user made a mistake on creation)
            if (!string.IsNullOrWhiteSpace(request.Type))
                category.Type = request.Type.ToUpper();

            if (!string.IsNullOrWhiteSpace(request.Icon))
                category.Icon = request.Icon;

            if (!string.IsNullOrWhiteSpace(request.Color))
                category.Color = request.Color;

            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task DeleteCategoryAsync(int userId, int categoryId)
        {
            var category = await GetUserOwnedCategoryAsync(userId, categoryId);

            bool hasTransactions = await _context.Transactions
                .AnyAsync(t => t.CategoryId == categoryId);

            if (hasTransactions)
                throw new InvalidOperationException(
                    "Cannot delete a category that has transactions. Reassign them first.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        // Private Helpers
        private async Task<Category> GetUserOwnedCategoryAsync(int userId, int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);

            if (category is null)
                throw new KeyNotFoundException("Category not found.");

            if (category.UserId is null)
                throw new UnauthorizedAccessException("System default categories cannot be modified.");

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this category.");

            return category;
        }

        private static CategoryDto MapToDto(Category category) => new()
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,                              
            Icon = category.Icon,
            Color = category.Color,
            IsSystem = category.UserId is null
        };
    }
}