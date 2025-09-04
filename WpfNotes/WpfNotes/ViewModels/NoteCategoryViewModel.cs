using System.ComponentModel;
using WpfNotes.Models.Note;

namespace WpfNotes.ViewModels
{
    public class NoteCategoryViewModel : CategoryViewModel
    {
        private readonly NoteCategoryModel _model;

        private Category _category;

        public Category Category 
        { 
            get => _category; 
            set { _category = value; OnPropertyChanged(nameof(Category)); } 
        }

        public NoteCategoryViewModel(Category category, bool isEmptyCategory) : base(isEmptyCategory)  
        {

            _model = new NoteCategoryModel(category);
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

        protected override async void DeleteCategory(object obj)
        {
            if (_confirmService.ShowConfirmation("Confirm", "Delete category"))
            {
                await _model.DeleteCategory();
                CloseWindow();
            }
        }

        protected override void CancelChanges(object obj)
        {
            _model.CancelChanges();
            CloseWindow();
        }

        protected override async void SaveCategory(object obj)
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
