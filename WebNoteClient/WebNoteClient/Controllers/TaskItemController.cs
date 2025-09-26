using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebNoteClient.Models;
using WebNoteClient.Models.TaskItem;
using WebNoteClient.Services;

namespace WebNoteClient.Controllers
{
    [Authorize]
    public class TaskItemController : Controller
    {
        private readonly ApiService _apiService;

        public TaskItemController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: TaskController
        public async Task<IActionResult> Index(int? categoryId, string? searchText)
        {
            var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
            if (accessTokenClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }            
            var categories = await _apiService.GetTaskCategoriesAsync(accessTokenClaim.Value);
            IEnumerable<TaskItemModel> tasks = await _apiService.GetTasksAsync(accessTokenClaim.Value);
            if (categoryId != null)
            {
                tasks = tasks.Where(t => t.Category.Id == categoryId);
            }
            if (searchText != null)
            {
                tasks = tasks.Where(t => t.Content.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }
            tasks = tasks.OrderByDescending(x => x.CreationDate);
            var model = new TaskPageViewModel
            {
                TaskItems = tasks.ToList(),
                Categories = categories,
                SelectedCategoryId = categoryId,
                SearchQuery = searchText,
            };
            return View(model);
        }

        // GET: TaskController/Create
        public async Task<IActionResult> Create()
        {
            var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
            if (accessTokenClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            
            var categories = await _apiService.GetTaskCategoriesAsync(accessTokenClaim.Value);
            var model = new CreateTaskViewModel
            {
                TaskItem = new TaskItemModel(),
                Categories = categories
            };
            return View(model);
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItemModel taskItem)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                await _apiService.CreateTaskItemAsync(accessTokenClaim.Value, taskItem);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Error));
            }
        }

        // GET: TaskController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
            if (accessTokenClaim == null) 
            {
                return RedirectToAction("Login", "Auth");
            }
            var task = await _apiService.GetTaskItemAsync(accessTokenClaim.Value, id);
            var categories = await _apiService.GetTaskCategoriesAsync(accessTokenClaim.Value);
            var model = new EditTaskViewModel 
            {
                TaskItem = task,
                Categories = categories 
            };
            return View(model);
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, TaskItemModel taskItem)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                await _apiService.UpdateTaskItemAsync(accessTokenClaim.Value, taskItem);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Error));
            }
        }

        // POST: TaskController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                await _apiService.DeleteTaskItemAsync(accessTokenClaim.Value, id);
                return RedirectToAction(nameof(Index), new { message = "Task was deleted successfully." });
            }
            catch
            {
                return RedirectToAction(nameof(Error));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                await _apiService.ToggleTaskCompleteAsync(accessTokenClaim.Value, id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Error));
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreateCategory([FromBody] TaskCategoryModel model)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    throw new AccessViolationException("Access denied");
                }

                await _apiService.CreateTaskCategoryAsync(accessTokenClaim.Value, model);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCategory([FromBody] TaskCategoryModel model)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    throw new AccessViolationException("Access denied");
                }

                await _apiService.UpdateTaskCategoryAsync(accessTokenClaim.Value, model);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                await _apiService.DeleteTaskCategoryAsync(accessTokenClaim.Value, id);
                return RedirectToAction(nameof(Index), new { message = "Category was deleted successfully." });
            }
            catch
            {
                return RedirectToAction(nameof(Error));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
