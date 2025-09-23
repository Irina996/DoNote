using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using WebNoteClient.Models;
using WebNoteClient.Models.Note;
using WebNoteClient.Services;

namespace WebNoteClient.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private readonly ApiService _apiService;

        public NoteController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: NoteController
        public async Task<IActionResult> Index(int? categoryId, string? searchText)
        {
            var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
            if (accessTokenClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            IEnumerable<NoteModel> notes = await _apiService.GetNotesAsync(accessTokenClaim.Value);
            var categories = await _apiService.GetNoteCategoriesAsync(accessTokenClaim.Value);
            if (categoryId != null)
            {
                notes = notes.Where(n => n.Category.Id == categoryId);
            }
            if (searchText != null)
            {
                notes = notes
                    .Where(n => n.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) 
                    || n.Content.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }
            notes = notes
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.ChangeDate);
            var model = new NotePageViewModel
            {
                Categories = categories,
                Notes = notes.ToList(),
                SelectedCategoryId = categoryId,
                SearchQuery = searchText,
            };
            return View(model);
        }

        // GET: NoteController/Create
        public async Task<IActionResult> Create()
        {
            var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
            if (accessTokenClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var categories = await _apiService.GetNoteCategoriesAsync(accessTokenClaim.Value);
            NoteModel note = new NoteModel();
            return View(new CreateNoteViewModel { Categories = categories, Note = note });
        }

        // POST: NoteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NoteModel note)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                await _apiService.CreateNoteAsync(accessTokenClaim.Value, note);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Error));
            }
        }

        // GET: NoteController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
            if (accessTokenClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var categories = await _apiService.GetNoteCategoriesAsync(accessTokenClaim.Value);
            NoteModel note = await _apiService.GetNote(accessTokenClaim.Value, (int)id);
            if (note == null)
            {
                throw new InvalidOperationException("Note does not exist or access denied");
            }
            return View(new EditNoteViewModel { Categories = categories, Note = note});
        }

        // POST: NoteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(NoteModel note)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                await _apiService.UpdateNoteAsync(accessTokenClaim.Value, note);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Error));
            }
        }

        // POST: NoteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                await _apiService.DeleteNoteAsync(accessTokenClaim.Value, id);
                return RedirectToAction(nameof(Index), new { message = "Note was deleted successfully." });
            }
            catch
            {
                return RedirectToAction(nameof(Error));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePin(int id)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                await _apiService.ToggleNotePinAsync(accessTokenClaim.Value, id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Error));
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreateCategory([FromBody] NoteCategoryModel model)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    throw new AccessViolationException("Access denied");
                }

                await _apiService.CreateNoteCategoryAsync(accessTokenClaim.Value, model);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCategory([FromBody] NoteCategoryModel model)
        {
            try
            {
                var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
                if (accessTokenClaim == null)
                {
                    throw new AccessViolationException("Access denied");
                }

                await _apiService.UpdateNoteCategoryAsync(accessTokenClaim.Value, model);

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
                await _apiService.DeleteNoteCategoryAsync(accessTokenClaim.Value, id);
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
