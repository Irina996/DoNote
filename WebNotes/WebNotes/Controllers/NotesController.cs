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
    public class NotesController : ControllerBase {

        ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotesController(ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNote([FromBody]CreateNoteModel model) {
            var user = await _userManager.GetUserAsync(User);
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId && c.UserId == user.Id);
            if (category == null)
            {
                return BadRequest(new { message = "No such category" });
            }

            // get note from request
            var note = new Note
            {
                Title = model.Title,
                Content = model.Content,
                CreationDate = DateTime.UtcNow,
                ChangeDate = DateTime.UtcNow,
                IsPinned = model.IsPinned,
                CategoryId = model.CategoryId
            };

            // save note to db
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            var noteModel = mapToNoteModel(note);

            // send note to user
            return CreatedAtAction(
                nameof(GetNote),
                new { id = note.Id },
                noteModel
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote(int id) {
            var user = await _userManager.GetUserAsync(User);
            var note = await _context.Notes.Include(n => n.Category)
                .FirstOrDefaultAsync(x => x.Id == id && x.Category.UserId == user.Id);
            if (note == null)
            {
                return NotFound();
            }
            var noteModel = mapToNoteModel(note);
            return Ok(noteModel);
        }

        private NoteModel mapToNoteModel(Note note) {
            return new NoteModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreationDate = note.CreationDate,
                ChangeDate = note.ChangeDate,
                IsPinned = note.IsPinned,
                Category = new CategoryModel
                {
                    Id = note.Category.Id,
                    Name = note.Category.Name,
                }
            };
        }
    }
}
