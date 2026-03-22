using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Extensions;
using ExpenseTrackerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All category endpoints require a valid JWT
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET /api/categories
        // Returns system defaults + user's own custom categories
        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var userId = User.GetUserId();
            var categories = await _categoryService.GetCategoriesAsync(userId);
            return Ok(categories);
        }

        // POST /api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var category = await _categoryService.CreateCategoryAsync(userId, request);

                // 201 Created with Location header pointing to GET /api/categories
                return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // 409 — duplicate name
            }
        }

        // PUT /api/categories/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var category = await _categoryService.UpdateCategoryAsync(userId, id, request);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); // 403 — trying to edit system/another user's category
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // DELETE /api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var userId = User.GetUserId();
                await _categoryService.DeleteCategoryAsync(userId, id);
                return NoContent(); // 204 — success, nothing to return
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // 409 — category has linked transactions
            }
        }
    }
}