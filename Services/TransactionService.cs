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
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(
            int userId, TransactionFilterParams filters)
        {
            // Start with base query — always scoped to the current user
            var query = _context.Transactions
                .Where(t => t.UserId == userId)
                .AsQueryable();

            // Dynamically apply filters only when provided
            if (filters.StartDate.HasValue)
                query = query.Where(t => t.Date >= filters.StartDate.Value);

            if (filters.EndDate.HasValue)
                query = query.Where(t => t.Date <= filters.EndDate.Value.AddDays(1).AddTicks(-1));
            // AddDays(1).AddTicks(-1) makes endDate INCLUSIVE of the full day (23:59:59)

            if (!string.IsNullOrWhiteSpace(filters.Type))
                query = query.Where(t => t.Type == filters.Type.ToUpper());

            if (filters.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == filters.CategoryId.Value);

            // Get total count BEFORE pagination for metadata
            var totalCount = await query.CountAsync();

            // Apply ordering and pagination AFTER filtering
            var transactions = await query
                .Include(t => t.Category)              // Join category for flattened DTO
                .OrderByDescending(t => t.Date)        // Most recent first
                .ThenByDescending(t => t.CreatedAt)
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Select(t => MapToDto(t))
                .ToListAsync();

            return new PagedResult<TransactionDto>
            {
                Data = transactions,
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<TransactionDto> GetTransactionByIdAsync(int userId, int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

            if (transaction is null)
                throw new KeyNotFoundException("Transaction not found.");

            return MapToDto(transaction);
        }

        public async Task<TransactionDto> CreateTransactionAsync(
            int userId, CreateTransactionRequest request)
        {
            // Validate category exists and belongs to this user's scope
            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.Id == request.CategoryId &&
                    (c.UserId == null || c.UserId == userId));

            if (category is null)
                throw new KeyNotFoundException("Category not found.");

            // Guard: transaction type must match the category type
            if (category.Type != request.Type.ToUpper())
                throw new InvalidOperationException(
                    $"Category '{category.Name}' is for {category.Type} transactions only. " +
                    $"Cannot use it for an {request.Type.ToUpper()} transaction.");

            var transaction = new Transaction
            {
                UserId = userId,
                CategoryId = request.CategoryId,
                Amount = request.Amount,
                Type = request.Type.ToUpper(),
                Currency = request.Currency.ToUpper(),
                Note = request.Note,
                Date = request.Date,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Reload with category for the DTO
            await _context.Entry(transaction)
                .Reference(t => t.Category)
                .LoadAsync();

            return MapToDto(transaction);
        }

        public async Task<TransactionDto> UpdateTransactionAsync(
            int userId, int transactionId, UpdateTransactionRequest request)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

            if (transaction is null)
                throw new KeyNotFoundException("Transaction not found.");

            // If CategoryId is changing, validate the new category
            if (request.CategoryId.HasValue && request.CategoryId.Value != transaction.CategoryId)
            {
                var newCategory = await _context.Categories
                    .FirstOrDefaultAsync(c =>
                        c.Id == request.CategoryId.Value &&
                        (c.UserId == null || c.UserId == userId));

                if (newCategory is null)
                    throw new KeyNotFoundException("Category not found.");

                // Determine the effective type after this update
                var effectiveType = request.Type?.ToUpper() ?? transaction.Type;

                if (newCategory.Type != effectiveType)
                    throw new InvalidOperationException(
                        $"Category '{newCategory.Name}' is for {newCategory.Type} transactions only.");

                transaction.CategoryId = request.CategoryId.Value;
                transaction.Category = newCategory;
            }

            // If only Type is changing, check it still matches current category
            if (!string.IsNullOrWhiteSpace(request.Type) &&
                request.Type.ToUpper() != transaction.Type &&
                !request.CategoryId.HasValue)
            {
                if (transaction.Category!.Type != request.Type.ToUpper())
                    throw new InvalidOperationException(
                        $"Cannot change type to {request.Type.ToUpper()}. " +
                        $"Current category '{transaction.Category.Name}' is for " +
                        $"{transaction.Category.Type} transactions. Change the category too.");

                transaction.Type = request.Type.ToUpper();
            }

            // Apply remaining optional updates
            if (request.Amount.HasValue)
                transaction.Amount = request.Amount.Value;

            if (!string.IsNullOrWhiteSpace(request.Currency))
                transaction.Currency = request.Currency.ToUpper();

            if (request.Note is not null)
                transaction.Note = request.Note;

            if (request.Date.HasValue)
                transaction.Date = request.Date.Value;

            await _context.SaveChangesAsync();

            return MapToDto(transaction);
        }

        public async Task DeleteTransactionAsync(int userId, int transactionId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

            if (transaction is null)
                throw new KeyNotFoundException("Transaction not found.");

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        // Private Helpers
        private static TransactionDto MapToDto(Transaction t) => new()
        {
            Id = t.Id,
            Amount = t.Amount,
            Type = t.Type,
            Currency = t.Currency,
            Note = t.Note,
            Date = t.Date,
            CreatedAt = t.CreatedAt,
            CategoryId = t.CategoryId,
            CategoryName = t.Category?.Name ?? string.Empty,
            CategoryIcon = t.Category?.Icon ?? string.Empty,
            CategoryColor = t.Category?.Color ?? string.Empty
        };
    }
}