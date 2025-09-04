using System.Windows.Input;
using WpfNotes.Services;

namespace WpfNotes.ViewModels
{
    public abstract class CategoryViewModel : ViewModelBase, IChangeWindows
    {
        protected readonly IConfirmService _confirmService;

        protected bool IsNewCategory;

        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }
        public Action Change { get; set; }

        protected CategoryViewModel(bool isNew)
        {
            _confirmService = new ConfirmService();
            IsNewCategory = isNew;

            DeleteCommand = new ViewModelCommand(DeleteCategory);
            CancelCommand = new ViewModelCommand(CancelChanges);
            SaveCommand = new ViewModelCommand(SaveCategory);
        }

        protected abstract void DeleteCategory(object obj);
        protected abstract void CancelChanges(object obj);
        protected abstract void SaveCategory(object obj);

        protected void CloseWindow()
        {
            Change?.Invoke();
        }
    }
}
