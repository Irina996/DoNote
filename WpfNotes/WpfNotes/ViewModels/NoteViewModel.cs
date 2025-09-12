using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using WpfNotes.Commands;
using WpfNotes.Models.Note;
using WpfNotes.Services;

namespace WpfNotes.ViewModels
{
    public class NoteViewModel : ViewModelBase, IChangeWindows
    {
        private readonly NoteModel _noteModel;
        private readonly IConfirmService _confirmService;

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
        public ICommand TogglePinCommand { get; }

        public Action Change { get; set; }

        public NoteViewModel(Note note, List<Category> categories, bool isNewNote)
        {
            _noteModel = new NoteModel(note, categories);
            _noteModel.PropertyChanged += OnNotePropertyChanged;
            _categories = new ObservableCollection<Category>(_noteModel.Categories);
            _note = _noteModel.Note;

            _confirmService = new ConfirmService();

            SaveCommand = new AsyncRelayCommand(SaveNote);
            DeleteCommand = new AsyncRelayCommand(DeleteNote);
            CancelCommand = new RelayCommand(Cancel);
            BackCommand = new RelayCommand(Back);
            TogglePinCommand = new AsyncRelayCommand(TogglePinNote);
            this.isNewNote = isNewNote;
        }

        private void OnNotePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Note))
            {
                Note = _noteModel.Note;
            }
        }

        private async Task SaveNote()
        {
            if (_confirmService.ShowConfirmation("Confirm", "Save the note?"))
            {
                if (isNewNote)
                {
                    await _noteModel.CreateNote();
                    isNewNote = false;
                }
                else
                {
                    await _noteModel.UpdateNote();
                }
            }
        }

        private async Task DeleteNote()
        {
            if (_confirmService.ShowConfirmation("Confirm", "Delete the note?"))
            {
                if (!isNewNote)
                {
                    await _noteModel.DeleteNote();
                }
                CloseWindow();
            }
        }

        private async Task TogglePinNote()
        {
            if (!isNewNote)
            {
                await _noteModel.TogglePin();
            }
            else
            {
                Note.IsPinned = !Note.IsPinned;
            }
        }

        private void Cancel()
        {
            if (_confirmService.ShowConfirmation("Confirm", "Cancel?"))
            {
                _noteModel.CancelChanges();
            }
        }

        private void Back()
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            Change?.Invoke();
        }
    }
}
