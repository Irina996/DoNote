using System.ComponentModel;
using WpfNotes.Models.TaskItem;

namespace WpfNotes.ViewModels
{
    public class TaskCategoryViewModel : CategoryViewModel
    {
        private readonly TaskCategoryModel _model;

        private TaskCategory _category;

        public TaskCategory Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }

        public TaskCategoryViewModel(TaskCategory category, bool isEmptyCategory) : base(isEmptyCategory) 
        {
            _model = new TaskCategoryModel(category);
            _model.PropertyChanged += OnModelPropertyChanged;
            Category = _model.Category;
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Category))
            {
                Category = _model.Category;
            }
        }

        protected override async Task DeleteCategory()
        {
            if (_confirmService.ShowConfirmation("Confirm", "Delete category"))
            {
                await _model.DeleteCategory();
                CloseWindow();
            }
        }

        protected override void CancelChanges()
        {
            _model.CancelChanges();
            CloseWindow();
        }

        protected override async Task SaveCategory()
        {
            if (IsNewCategory)
            {
                await _model.CreateCategory();
                IsNewCategory = false;
            }
            else
            {
                await _model.UpdateCategory();
            }
            CloseWindow();
        }
    }
}
