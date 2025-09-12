using System.Windows.Input;
using WpfNotes.Commands;
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

            DeleteCommand = new AsyncRelayCommand(DeleteCategory);
            CancelCommand = new RelayCommand(CancelChanges);
            SaveCommand = new AsyncRelayCommand(SaveCategory);
        }

        protected abstract Task DeleteCategory();
        protected abstract void CancelChanges();
        protected abstract Task SaveCategory();

        protected void CloseWindow()
        {
            Change?.Invoke();
        }
    }
}
