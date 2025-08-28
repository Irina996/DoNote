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

namespace WpfNotes.ViewModels
{
    public class NotesListViewModel : ViewModelBase
    {
        private NotesListModel _notesModel = new NotesListModel();

        private ObservableCollection<Note> _notes;
        private ObservableCollection<Category> _categories;
        private Note _selectedNote;
        private Category _selectedCategory;
        private string _searchText;

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

        public ICommand CreateNoteCommand { get; }
        public ICommand EditNoteCommand { get;  }

        public ICommand CreateCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }

        public ICommand SearchNoteCommand {  get; }

        public ICommand ChangeSelectedCategoryCommand { get; }

        public Action<Note, IEnumerable<Category>> OpenNoteWindowAction { get; set; }
        public Action<Category> OpenCategoryWindowAction { get; set; }

        public NotesListViewModel()
        {
            _notesModel.PropertyChanged += OnNotesModelPropertyChanged;

            CreateNoteCommand = new ViewModelCommand(CreateNote);
            EditNoteCommand = new ViewModelCommand(EditNote);
            CreateCategoryCommand = new ViewModelCommand(CreateCategory);
            EditCategoryCommand = new ViewModelCommand(EditCategory);
            SearchNoteCommand = new ViewModelCommand(SearchNote);
            ChangeSelectedCategoryCommand = new ViewModelCommand(ChangeCategory);

            _notes = new ObservableCollection<Note>();
            _categories = new ObservableCollection<Category>();
            _searchText = "";

            LoadData();
        }

        private async void LoadData()
        {
            await _notesModel.LoadAsync();
        }

        private void OnNotesModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NotesListModel.Notes))
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
                Categories.Clear();
                // add "All" category
                Categories.Add(new Category {Id = 0, Name = "All"});
                SelectedCategory = Categories[0];
                foreach (var category in _notesModel.Categories)
                {
                    Categories.Add(category);
                }
                OnPropertyChanged(nameof(Categories));
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        private void OpenNoteWindow(Note note)
        {
            List<Category> categories = new List<Category>(_categories);
            categories.RemoveAt(0); // remove "All" category
            OpenNoteWindowAction?.Invoke(note, categories);
        }

        private void OpenCategoryWindow(Category category)
        {
            OpenCategoryWindowAction?.Invoke(category);
        }

        private void CreateNote(object obj)
        {
            OpenNoteWindow(new Note());
        }

        private void EditNote(object obj)
        {
            OpenNoteWindow(SelectedNote);
        }

        private void CreateCategory(object obj)
        {
            Category category = new Category();
            OpenCategoryWindow(category);
        }

        private void EditCategory(object obj)
        {
            if (SelectedCategory.Id != 0)
            {
                OpenCategoryWindow(SelectedCategory);
            }
        }

        private void SearchNote(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                _notesModel.GetAllNotes();
            }
            else 
            { 
                _notesModel.FindNotes(SearchText);
            }
        }

        private void ChangeCategory(object obj)
        {
            if (SelectedCategory.Id == 0)
            {
                // All category
                _notesModel.GetAllNotes();
            }
            else 
            { 
                _notesModel.FilterNotes(SelectedCategory); 
            }
        }
    }
}
