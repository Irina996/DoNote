using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfNotes.Models.Note;
using WpfNotes.Services;

namespace WpfNotes.ViewModels
{
    public class NotesListViewModel : ViewModelBase
    {
        private readonly NotesListModel _notesModel;
        private readonly IWindowService _windowService;

        private ObservableCollection<Note> _allNotes;
        private ObservableCollection<Note> _notes;
        private ObservableCollection<Category> _categories;
        private Note _selectedNote;
        private Category _selectedCategory;
        private string _searchText;
        private bool _isLoading;

        public ObservableCollection<Note> AllNotes 
        {
            get => _allNotes;
            set
            {
                _allNotes = value;
                OnPropertyChanged(nameof(AllNotes));
            }
        }
        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set 
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }
        public Note SelectedNote
        {
            get => _selectedNote;
            set
            {
                _selectedNote = value;
                OnPropertyChanged(nameof(SelectedNote));
            }
        }
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value; 
                OnPropertyChanged(nameof(SelectedCategory));  
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }
        public bool IsLoading
        {
            get => _isLoading;
            set
            {  
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand CreateNoteCommand { get; }
        public ICommand EditNoteCommand { get;  }
        public ICommand CreateCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand SearchNoteCommand {  get; }
        public ICommand ChangeSelectedCategoryCommand { get; }

        public NotesListViewModel()
        {
            _notesModel = new NotesListModel();
            _notesModel.PropertyChanged += OnNotesModelPropertyChanged;

            _windowService = new WindowService();

            LoadData(null);
            LoadCommand = new ViewModelCommand(LoadData);
            CreateNoteCommand = new ViewModelCommand(CreateNote);
            EditNoteCommand = new ViewModelCommand(EditNote);
            CreateCategoryCommand = new ViewModelCommand(CreateCategory);
            EditCategoryCommand = new ViewModelCommand(EditCategory);
            SearchNoteCommand = new ViewModelCommand(FilterNotes);
            ChangeSelectedCategoryCommand = new ViewModelCommand(FilterNotes);

            _allNotes = new ObservableCollection<Note>();
            _notes = new ObservableCollection<Note>();
            _categories = new ObservableCollection<Category>();
            _searchText = "";
        }

        private void OnNotesModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NotesListModel.AllNotes))
            {
                AllNotes.Clear();
                foreach (var note in _notesModel.AllNotes)
                {
                    AllNotes.Add(note);
                }
            }
            else if (e.PropertyName == nameof(NotesListModel.Notes))
            {
                Notes.Clear();
                foreach (var note in _notesModel.Notes)
                {
                    Notes.Add(note);
                }
                SelectedNote = Notes.FirstOrDefault();
                OnPropertyChanged(nameof(Notes));
                OnPropertyChanged(nameof(SelectedNote));
            }
            else if (e.PropertyName == nameof(NotesListModel.Categories))
            {
                _categories.Clear();
                // add "All" category
                _categories.Add(new Category {Id = -1, Name = "All"});
                _selectedCategory = Categories[0];
                foreach (var category in _notesModel.Categories)
                {
                    _categories.Add(category);
                }
                OnPropertyChanged(nameof(Categories));
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        private async void LoadData(object obj)
        {
            IsLoading = true;
            await _notesModel.LoadAsync();
            IsLoading = false;
        }

        private async Task OpenNoteWindow(Note note, bool isNewNote)
        {
            List<Category> categories = new List<Category>(_categories);
            categories.RemoveAt(0); // remove "All" category
            _windowService.ShowNoteWindow(new NoteViewModel(note, categories, isNewNote));
            if (isNewNote && !string.IsNullOrEmpty(note.Title))
            {
                _notesModel.AddNote(note);
            }
            if (!isNewNote && string.IsNullOrEmpty(note.Title))
            {
                _notesModel.RemoveNote(note);
            }
            FilterNotes(null);
        }

        private async Task OpenCategoryWindowAsync(Category category, bool isNewCategory)
        {
            _windowService.ShowCategoryWindow(new NoteCategoryViewModel(category, isNewCategory));
            if (isNewCategory && !string.IsNullOrEmpty(category.Name))
            {
                Categories.Add(category);
            }
            if (!isNewCategory && string.IsNullOrEmpty(category.Name))
            {
                Categories.Remove(category);
            }
        }

        private void CreateNote(object obj)
        {
            var note = new Note();
            if (SelectedCategory.Id == -1)
            {
                note.Category = Categories[1];
            }
            else
            {
                note.Category = SelectedCategory;
            }
            OpenNoteWindow(note, true);
        }

        private void EditNote(object obj)
        {
            OpenNoteWindow(SelectedNote, false);
        }

        private void CreateCategory(object obj)
        {
            Category category = new Category();
            OpenCategoryWindowAsync(category, true);
        }

        private void EditCategory(object obj)
        {
            if (SelectedCategory.Id != -1)
            {
                OpenCategoryWindowAsync(SelectedCategory, false);
            }
        }

        private void FilterNotes(object obj)
        {
            SelectedCategory ??= Categories[0];
            if (SelectedCategory.Id == -1)
            {
                // All category
                _notesModel.FilterNotes(SearchText);
            }
            else
            {
                _notesModel.FilterNotes(SearchText, SelectedCategory);
            }
        }
    }
}
