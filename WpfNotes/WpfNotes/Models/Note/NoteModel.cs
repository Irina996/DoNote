using System;
using System.Collections.Generic;
using System.Linq;
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
            Note = note;
            if (Note.Category == null)
            {
                Note.Category = categories[0];
            }
            else
            {
                Note.Category = categories.First(c => c.Id == Note.Category.Id);
            }
            Categories = categories;
            CopyNoteVersion();
        }

        private  void CopyNoteVersion()
        {
            _prevNote = new Note
            {
                Id = Note.Id,
                Title = Note.Title,
                Content = Note.Content,
                Category = Note.Category,
                ChangeDate = Note.ChangeDate,
                CreationDate = Note.CreationDate,
                IsPinned = Note.IsPinned
            };
        }

        public async void CreateNote()
        {
            var note = await _apiService.CreateNoteAsync(Note);
            Note = note;
            CopyNoteVersion();
        }

        public async void UpdateNote()
        {
            await _apiService.UpdateNoteAsync(Note);
            CopyNoteVersion();
        }

        public async void DeleteNote()
        {
            await _apiService.DeleteNoteAsync(Note);
            Note = null;
        }

        public void CancelChanges()
        {
            Note = _prevNote;
            CopyNoteVersion();
        }
    }
}
