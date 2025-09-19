using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index(int? categoryId)
        {
            var accessTokenClaim = HttpContext.User.FindFirst("AccessToken");
            if (accessTokenClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var notes = await _apiService.GetNotesAsync(accessTokenClaim.Value);
            var categories = await _apiService.GetNoteCategoriesAsync(accessTokenClaim.Value);
            if (categoryId != null)
            {
                notes = notes.Where(n => n.Category.Id == categoryId).ToList();
            }
            notes = notes
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.ChangeDate)
                .ToList();
            var model = new NotePageViewModel
            {
                Categories = categories,
                Notes = notes,
                SelectedCategoryId = categoryId,
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
                return View();
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
        public async Task<ActionResult> Edit(int id, NoteModel note)
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
                return View();
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
                return RedirectToAction(nameof(Index), new { message = "Note deleted successfully." });
            }
            catch
            {
                return View();
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
                return View();
            }
        }
    }
}
