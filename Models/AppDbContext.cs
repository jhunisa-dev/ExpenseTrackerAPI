using System.Collections.Generic;
using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // These DbSets represent the actual tables in your SQL database
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                // ✅ EXPENSE defaults
                new Category { Id = 1, Name = "Food & Dining", Type = "EXPENSE", Icon = "🍔", Color = "#F59E0B", UserId = null },
                new Category { Id = 2, Name = "Transportation", Type = "EXPENSE", Icon = "🚗", Color = "#3B82F6", UserId = null },
                new Category { Id = 3, Name = "Housing", Type = "EXPENSE", Icon = "🏠", Color = "#8B5CF6", UserId = null },
                new Category { Id = 4, Name = "Healthcare", Type = "EXPENSE", Icon = "🏥", Color = "#EF4444", UserId = null },
                new Category { Id = 5, Name = "Entertainment", Type = "EXPENSE", Icon = "🎮", Color = "#EC4899", UserId = null },
                new Category { Id = 6, Name = "Shopping", Type = "EXPENSE", Icon = "🛍️", Color = "#14B8A6", UserId = null },
                new Category { Id = 7, Name = "Utilities", Type = "EXPENSE", Icon = "⚡", Color = "#F97316", UserId = null },
                new Category { Id = 8, Name = "Education", Type = "EXPENSE", Icon = "📚", Color = "#6366F1", UserId = null },

                // ✅ INCOME defaults
                new Category { Id = 9, Name = "Salary", Type = "INCOME", Icon = "💼", Color = "#10B981", UserId = null },
                new Category { Id = 10, Name = "Freelance", Type = "INCOME", Icon = "💻", Color = "#06B6D4", UserId = null },
                new Category { Id = 11, Name = "Investments", Type = "INCOME", Icon = "📈", Color = "#84CC16", UserId = null },

                // ✅ Neutral (usable for either, but classified as EXPENSE by convention)
                new Category { Id = 12, Name = "Other", Type = "EXPENSE", Icon = "📦", Color = "#6B7280", UserId = null }
            );
        }
    }
}