using ExpenseTrackerAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTrackerAPI.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategoriesAsync(int userId);
        Task<CategoryDto> CreateCategoryAsync(int userId, CreateCategoryRequest request);
        Task<CategoryDto> UpdateCategoryAsync(int userId, int categoryId, UpdateCategoryRequest request);
        Task DeleteCategoryAsync(int userId, int categoryId);
    }
}