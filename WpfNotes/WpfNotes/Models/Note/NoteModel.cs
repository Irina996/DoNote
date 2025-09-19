using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Services;

namespace WpfNotes.Models.Note
{
    public class NoteModel : BindableBase
    {
        private readonly ApiService _apiService;

        private Note _prevNote;
        private Note _note;
        private List<Category> _categories;

        public Note Note
        {
            get => _note;
            set { SetProperty(ref _note, value); }
        }
        public List<Category> Categories
        {
            get => _categories;
            set {  SetProperty(ref _categories, value); }
        }

        public NoteModel()
        {
            _apiService = ApiService.GetInstance();
        }

        public NoteModel(Note note, List<Category> categories) : this()
        {
            _prevNote = note;
            if (_prevNote.Category == null)
            {
                _prevNote.Category = categories[0];
            }
            else
            {
                _prevNote.Category = categories.First(c => c.Id == _prevNote.Category.Id);
            }
            Categories = categories;
            CopyNoteVersion();
        }

        private void CopyNoteVersion()
        {
            Note = new Note
            {
                Id = _prevNote.Id,
                Title = _prevNote.Title,
                Content = _prevNote.Content,
                Category = _prevNote.Category,
                ChangeDate = _prevNote.ChangeDate,
                CreationDate = _prevNote.CreationDate,
                IsPinned = _prevNote.IsPinned,
            };
        }

        private void UpdatePrevVersion(Note note)
        {
            _prevNote.Title = note.Title;
            _prevNote.Content = note.Content;
            _prevNote.Category = note.Category;
            _prevNote.ChangeDate = note.ChangeDate;
            _prevNote.IsPinned = note.IsPinned;
        }

        public async Task CreateNote()
        {
            var note = await _apiService.CreateNoteAsync(Note);
            UpdatePrevVersion(note);
            CopyNoteVersion();
        }

        public async Task UpdateNote()
        {
            await _apiService.UpdateNoteAsync(Note);
            UpdatePrevVersion(Note);
            CopyNoteVersion();
        }

        public async Task DeleteNote()
        {
            await _apiService.DeleteNoteAsync(Note);
            _prevNote.Title = null;
            _prevNote.Content = null;
            Note = null;
        }

        public async Task TogglePin()
        {
            _prevNote.IsPinned = !_prevNote.IsPinned;
            await _apiService.TogglePinNote(_prevNote);
            _note.IsPinned = _prevNote.IsPinned;
        }

        public void CancelChanges()
        {
            CopyNoteVersion();
        }
    }
}
