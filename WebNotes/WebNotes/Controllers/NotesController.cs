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

        /// <summary>
        /// Creates a new note for the authenticated user.
        /// </summary>
        /// <param name="model">The data required to create a note, including title, content, and category.</param>
        /// <returns>The created note with its assigned ID and metadata.</returns>
        /// <response code="201">Returns the newly created note.</response>
        /// <response code="400">If the model is invalid or the specified category does not exist or belong to the user.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody]CreateNoteModel model) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            // Find the category and ensure it belongs to the user 
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId && c.UserId == user.Id);
            if (category == null)
            {
                return BadRequest(new { message = "Category not found or access denied." });
            }

            // Create new note
            var note = new Note
            {
                Title = model.Title,
                Content = model.Content,
                CreationDate = DateTime.UtcNow,
                ChangeDate = DateTime.UtcNow,
                IsPinned = model.IsPinned,
                CategoryId = model.CategoryId
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            var noteModel = MapToNoteModel(note);

            return CreatedAtAction(
                nameof(GetNote),
                new { id = note.Id },
                noteModel
            );
        }

        /// <summary>
        /// Retrieves a specific note by its ID, if it belongs to the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the note to retrieve.</param>
        /// <returns>The requested note, if found and accessible.</returns>
        /// <response code="200">Returns the requested note.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the note does not exist or does not belong to the user.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote(int id) {
            var user = await _userManager.GetUserAsync(User);
            var note = await _context.Notes.Include(n => n.Category)
                .FirstOrDefaultAsync(x => x.Id == id && x.Category.UserId == user.Id);
            if (note == null)
            {
                return NotFound();
            }
            var noteModel = MapToNoteModel(note);
            return Ok(noteModel);
        }

        /// <summary>
        /// Retrieves a list of all notes belonging to the authenticated user.
        /// </summary>
        /// <returns>A list of notes with their associated category and metadata.</returns>
        /// <response code="200">Returns a list of the user's notes.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <remarks>
        /// TODO: Implement pagination, sorting, and filtering.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetNotes() {
            var user = await _userManager.GetUserAsync(User);
            var notesList = await _context.Notes.Include(n => n.Category)
                .Where(x => x.Category.UserId == user.Id).ToListAsync();
            var noteModelList = notesList.Select(MapToNoteModel).ToList();
            return Ok(noteModelList);
        }

        /// <summary>
        /// Updates an existing note if it belongs to the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the note to update.</param>
        /// <param name="model">The updated note data.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">Indicates the note was successfully updated.</response>
        /// <response code="400">If the model is invalid or the specified category does not exist or belong to the user.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the note does not exist or does not belong to the user.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody]UpdateNoteModel model) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            // Find the note and ensure it belongs to the user via category ownership
            var note = await _context.Notes.Include(n => n.Category)
                .FirstOrDefaultAsync(x => x.Id == id && x.Category.UserId == user.Id);
            if (note == null)
            {
                return NotFound(new { message = "Note not found or access denied." });
            }

            // Find new category and ensure it belongs to the user
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId && c.UserId == user.Id);
            if (category == null) {
                return BadRequest(new { message = "Category not found or access denied." });
            }

            // Update note fields
            note.Title = model.Title;
            note.Content = model.Content;
            note.IsPinned = model.IsPinned;
            note.CategoryId = model.CategoryId;
            note.ChangeDate = DateTime.UtcNow; // Set time of the last update

            _context.Notes.Update(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific note by its ID, if it belongs to the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the note to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        /// <response code="204">Indicates the note was successfully deleted.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the note does not exist or does not belong to the user.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id) {
            var user = await _userManager.GetUserAsync(User);

            var note = await _context.Notes.Include(n => n.Category)
                .FirstOrDefaultAsync(x => x.Id == id && x.Category.UserId == user.Id);
            if (note == null) {
                return NotFound(new { message = "Note not found or access denied." });
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Maps a Note entity to its corresponding NoteModel DTO for response serialization.
        /// </summary>
        /// <param name="note">The Note entity to map.</param>
        /// <returns>A NoteModel containing the note's data and associated category information.</returns>
        private NoteModel MapToNoteModel(Note note) {
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

        /// <summary>
        /// Update notes pin state
        /// </summary>
        /// <param name="id">Note id that needed to change pin state</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">Indicates the note was successfully updated.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the note does not exist or does not belong to the user.</response>
        [HttpPatch("{id}/pin")]
        public async Task<IActionResult> ToggleNotePin(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var note = await _context.Notes.Include(n => n.Category)
                .FirstOrDefaultAsync(n => n.Id == id && user.Id == n.Category.UserId);
            if (note == null)
            {
                return NotFound(new { message = "Note not found or access denied." });
            }

            note.IsPinned = !note.IsPinned;
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
