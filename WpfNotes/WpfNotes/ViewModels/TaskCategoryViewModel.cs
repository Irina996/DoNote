using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNotes.Models.TaskItem;
using WpfNotes.Services;

namespace WpfNotes.ViewModels
{
    public class TaskCategoryViewModel : CategoryViewModel, IChangeWindows
    {
        private readonly TaskCategoryModel _model;
        private readonly IConfirmService _confirmService;

        private TaskCategory _category;
        private bool isNewCategory;

        public TaskCategory Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }
        public Action Change { get; set; }

        public TaskCategoryViewModel(TaskCategory category, bool isEmptyCategory)
        {
            _model = new TaskCategoryModel(category);
            _model.PropertyChanged += OnModelPropertyChanged;
            Category = _model.Category;
            isNewCategory = isEmptyCategory;

            _confirmService = new ConfirmService();

            DeleteCommand = new ViewModelCommand(DeleteCategory);
            CancelCommand = new ViewModelCommand(CancelChanges);
            SaveCommand = new ViewModelCommand(SaveCategory);
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Category))
            {
                Category = _model.Category;
            }
        }

        private async void DeleteCategory(object obj)
        {
            if (_confirmService.ShowConfirmation("Confirm", "Delete category"))
            {
                await _model.DeleteCategory();
                CloseWindow();
            }
        }

        private void CancelChanges(object obj)
        {
            _model.CancelChanges();
            CloseWindow();
        }

        private async void SaveCategory(object obj)
        {
            if (isNewCategory)
            {
                await _model.CreateCategory();
                isNewCategory = false;
            }
            else
            {
                await _model.UpdateCategory();
            }
            CloseWindow();
        }

        private void CloseWindow()
        {
            Change?.Invoke();
        }
    }
}
