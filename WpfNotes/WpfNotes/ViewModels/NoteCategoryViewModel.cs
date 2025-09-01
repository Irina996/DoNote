using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNotes.Models.Note;

namespace WpfNotes.ViewModels
{
    class NoteCategoryViewModel : CategoryViewModel, IChangeWindows, IConfirm
    {
        private readonly NoteCategoryModel _model;

        private Category _category;

        public Category Category 
        { 
            get => _category; 
            set { _category = value; OnPropertyChanged(nameof(Category)); } 
        }

        private bool isNewCategory = true;
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }
        public Action Change { get; set; }
        public Predicate<string> Confirm { get; set; }

        public NoteCategoryViewModel(Category category, bool isEmptyCategory) 
        {
            isNewCategory = isEmptyCategory;

            _model = new NoteCategoryModel(category);
            _model.PropertyChanged += OnModelPropertyChanged;
            Category = _model.Category;

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
            if (Confirm?.Invoke("Delete category") ?? false)
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
