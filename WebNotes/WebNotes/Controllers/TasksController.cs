using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNotes.Data;
using WebNotes.Entities;
using WebNotes.Models;

namespace WebNotes.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TasksController(UserManager<IdentityUser> userManager, ApplicationDbContext context) {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of all tasks belonging to the authenticated user, including their categories.
        /// </summary>
        /// <returns>A list of task DTOs with full metadata.</returns>
        /// <response code="200">Returns the list of tasks.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet]
        public async Task<IActionResult> GetTaskItems() {
            var user = await _userManager.GetUserAsync(User);

            var tasks = await _context.TaskItems.Include(t => t.Category)
                .Where(t => t.Category.UserId == user.Id).ToListAsync();
            var taskModels = tasks.Select(MapToTaskModel).ToList();
            return Ok(taskModels);
        }

        /// <summary>
        /// Retrieves a specific task by its ID, if it belongs to the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the task to retrieve.</param>
        /// <returns>The requested task with its category details.</returns>
        /// <response code="200">Returns the requested task.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the task does not exist or does not belong to the user.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskItem(int id) {
            var user = await _userManager.GetUserAsync(User);

            var task = await _context.TaskItems.Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.Category.UserId == user.Id);
            if (task == null)
            {
                return NotFound(new { message = "Task not found or access denied." });
            }
            return Ok(MapToTaskModel(task));
        }

        /// <summary>
        /// Creates a new task for the authenticated user.
        /// </summary>
        /// <param name="model">The data required to create a task, including content and category.</param>
        /// <returns>The created task with its assigned ID and metadata.</returns>
        /// <response code="201">Returns the newly created task.</response>
        /// <response code="400">If the model is invalid or the specified category does not exist or belong to the user.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        public async Task<IActionResult> CreateTaskItem([FromBody]CreateTaskModel model) {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId && c.UserId == user.Id);
            if (category == null) 
            {
                return BadRequest(new {message = "Category not found or access denied."});
            }

            var task = new TaskItem
            {
                Content = model.Content,
                IsCompleted = false,
                Notification = model.Notification,
                CreationDate = DateTime.UtcNow,
                CategoryId = category.Id,
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTaskItem), 
                new {id = task.Id}, 
                MapToTaskModel(task)
            );
        }

        /// <summary>
        /// Updates an existing task if it belongs to the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the task to update.</param>
        /// <param name="model">The updated task data.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">Indicates the task was successfully updated.</response>
        /// <response code="400">If the model is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the task does not exist or does not belong to the user.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskItem(int id, [FromBody]UpdateTaskItem model) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            var task = await _context.TaskItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.Category.UserId == user.Id);
            if (task == null)
            {
                return NotFound(new { message = "Task not found or access denied." });
            }

            var category = await _context.TaskCategories
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId && c.UserId == user.Id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found or access denied." });
            }

            task.Content = model.Content;
            task.Notification = model.Notification;
            task.CategoryId = model.CategoryId;
            task.IsCompleted = model.IsCompleted;

            _context.TaskItems.Update(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific task by its ID, if it belongs to the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        /// <response code="204">Indicates the task was successfully deleted.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the task does not exist or does not belong to the user.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id) {
            var user = await _userManager.GetUserAsync(User);

            var task = await _context.TaskItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.Category.UserId == user.Id);
            if (task == null)
            {
                return NotFound(new { message = "Task not found or access denied." });
            }

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Maps a TaskItem entity to its corresponding TaskModel DTO for response serialization.
        /// </summary>
        /// <param name="task">The TaskItem entity to map.</param>
        /// <returns>A TaskModel containing the task's data and associated category information.</returns>
        private TaskModel MapToTaskModel(TaskItem task) {
            return new TaskModel
            {
                Id = task.Id,
                Content = task.Content,
                CreationDate = task.CreationDate,
                IsCompleted = task.IsCompleted,
                Notification = task.Notification,
                Category = new CategoryModel
                {
                    Id = task.Category.Id,
                    Name = task.Category.Name,
                }
            };
        }
    }
}
