using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNotes.Data;
using WebNotes.Models;
using WebNotes.Entities;

namespace WebNotes.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskCategoriesController : ControllerBase {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TaskCategoriesController(UserManager<IdentityUser> userManager, ApplicationDbContext context) {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Creates a new category for authenticated user
        /// </summary>
        /// <param name="model">The data required to create category, including name.</param>
        /// <returns>The created category.</returns>
        /// <response code="201">Returns the newly created category.</response>
        /// <response code="400">If the model is invalid or category already exists.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody]CreateCategoryModel model) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Name == model.Name);
            if (category != null)
            {
                return BadRequest(new { message = "Category with such name already exists" });
            }

            category = new TaskCategory { 
                Name = model.Name,
                UserId = user.Id
            };

            _context.TaskCategories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.Id },
                MapToTaskCategoryModel(category)
            );
        }

        /// <summary>
        /// Retrieves a specific category by its ID, if it belongs to the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>The requested category, if found and accessible.</returns>
        /// <response code="200">Returns the requested category.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the category does not exist or does not belong to the user.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id) {
            var user = await _userManager.GetUserAsync(User);
            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found or access denied." });
            }

            return Ok(MapToTaskCategoryModel(category));
        }

        /// <summary>
        /// Retrieves a list of all TasksCategories belonging to the authenticated user.
        /// </summary>
        /// <returns>A list of TasksCategories with their associated category and metadata.</returns>
        /// <response code="200">Returns a list of the user's TasksCategories.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet]
        public async Task<IActionResult> GetTasksCategories() {
            var user = await _userManager.GetUserAsync(User);
            var taskCategories = await _context.TaskCategories
                .Where(c => c.UserId == user.Id).ToListAsync();

            var categoryModelList = taskCategories
                .Select(MapToTaskCategoryModel).ToList();

            return Ok(categoryModelList);
        }

        /// <summary>
        /// Updates an existing category if it belongs to the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="model">The updated category data.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">Indicates the category was successfully updated.</response>
        /// <response code="400">If the model is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the category does not exist or does not belong to the user.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody]UpdateCategoryModel model) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found or access denied." });
            }

            category.Name = model.Name;

            _context.TaskCategories.Update(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific category by its id, if it belongs to the authenticated user.
        /// </summary>
        /// <param name="id">The id of the category to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        /// <response code="204">Indicates the category was successfully deleted.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the category does not exist or does not belong to the user.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id) {
            var user = await _userManager.GetUserAsync(User);

            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found or access denied." });
            }

            _context.TaskCategories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private CategoryModel MapToTaskCategoryModel(TaskCategory category) {
            return new CategoryModel
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
