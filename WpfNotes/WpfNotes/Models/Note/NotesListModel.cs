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

        public void FindNotes(string text)
        {
            Notes = _allNotes
                .Where(n => n.Title.ToLower().IndexOf(text.ToLower()) > -1 
                    || n.Content.ToLower().IndexOf(text.ToLower()) > -1)
                .OrderByDescending(n => n.ChangeDate)
                .ToList();
        }

        public void FilterNotes(Category category)
        {
            Notes = _allNotes
                .Where(n => n.Category.Id == category.Id)
                .OrderByDescending(n => n.ChangeDate)
                .ToList();
        }

        public void GetAllNotes()
        {
            Notes = _allNotes;
        }
    }
}
