using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNotes.Models.Note;

namespace WpfNotes.ViewModels
{
    public class NoteViewModel : ViewModelBase, IChangeWindows
    {
        private NoteModel _noteModel;

        private bool isNewNote;
        private Note _note;
        private ObservableCollection<Category> _categories;

        public Note Note
        {
            get => _note; set { _note = value; OnPropertyChanged(nameof(Note)); }
        }
        public ObservableCollection<Category> Categories
        {
            get => _categories; set { _categories = value; OnPropertyChanged(nameof(Categories)); }
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BackCommand { get; }

        public Action Change { get; set; }

        public NoteViewModel(Note note, List<Category> categories, bool isNewNote)
        {
            _noteModel = new NoteModel(note, categories);
            _noteModel.PropertyChanged += OnNotePropertyChanged;
            _categories = new ObservableCollection<Category>(_noteModel.Categories);
            _note = _noteModel.Note;

            SaveCommand = new ViewModelCommand(SaveNote);
            DeleteCommand = new ViewModelCommand(DeleteNote);
            CancelCommand = new ViewModelCommand(Cancel);
            BackCommand = new ViewModelCommand(Back);
            this.isNewNote = isNewNote;
        }

        private void OnNotePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Note))
            {
                Note = _noteModel.Note;
                OnPropertyChanged(nameof(Note));
            }
        }

        private void SaveNote(object obj)
        {
            if (isNewNote)
            {
                _noteModel.CreateNote();
                isNewNote = false;
            }
            else
            {
                _noteModel.UpdateNote();
            }
        }

        private void DeleteNote(object obj)
        {
            if (!isNewNote)
            {
                _noteModel.DeleteNote();
            }
            CloseWindow();
        }

        private void Cancel(object obj)
        {
            _noteModel.CancelChanges();
        }

        private void Back(object obj)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            Change?.Invoke();
        }
    }
}
