using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Services;

namespace WpfNotes.Models.Note
{
    public class NotesListModel : BindableBase
    {
        private readonly ApiService _apiService;

        private List<Note> _allNotes = new List<Note>();
        private List<Note> _notes = new List<Note>();
        private List<Category> _allCategories = new List<Category>();

        public List<Note> AllNotes
        {
            get => _allNotes; 
            set 
            { 
                SetProperty(ref _allNotes, value);
            }
        }
        public List<Note> Notes
        {
            get => _notes;
            set
            {
                SetProperty(ref _notes, value);
            }
        }
        public List<Category> Categories
        {
            get => _allCategories;
            set
            {
                SetProperty(ref _allCategories, value);
            }
        }

        public NotesListModel() 
        {
            _apiService = ApiService.GetInstance();
        }

        public async Task LoadAsync()
        {
            try
            {
                _allNotes = (await _apiService.GetNotesAsync())
                    .OrderByDescending(n => n.ChangeDate).ToList();
                Notes = _allNotes;

                var categories = await _apiService.GetNoteCategoriesAsync();
                Categories = categories;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        public void AddNote(Note note)
        {
            AllNotes.Insert(0, note);
        }

        public void RemoveNote(Note note)
        {
            AllNotes.Remove(note);
        }

        public void FilterNotes(string text = null, Category category = null)
        {
            IEnumerable<Note> filteredByText = AllNotes;
            if (!string.IsNullOrEmpty(text))
            {
                filteredByText = AllNotes.Where(n => n.Title.ToLower().IndexOf(text.ToLower()) > -1
                    || n.Content.ToLower().IndexOf(text.ToLower()) > -1)
                    .OrderByDescending(n => n.ChangeDate);
            }
            IEnumerable<Note> filteredByCategory = AllNotes;
            if (category != null)
            {
                filteredByCategory = AllNotes
                    .Where(n => n.Category.Id == category.Id)
                    .OrderByDescending(n => n.ChangeDate);
            }
            Notes = filteredByText.Intersect(filteredByCategory).ToList();
        }

        public void GetAllNotes()
        {
            Notes = _allNotes;
        }
    }
}
