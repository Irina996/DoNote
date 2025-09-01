using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Services;

namespace WpfNotes.Models.Note
{
    public class NoteCategoryModel : BindableBase
    {
        private readonly ApiService _apiService;

        private Category _prevCategory;
        private Category _category;

        public Category Category
        { 
            get =>  _category;
            set { SetProperty(ref _category, value); }
        }

        public NoteCategoryModel(Category category)
        {
            _apiService = ApiService.GetInstance();
            _prevCategory = category;
            CopyVersion();
        }

        public async Task<bool> CreateCategory()
        {
            Category = await _apiService.CreateNoteCategoryAsync(Category);
            _prevCategory.Id = Category.Id;
            _prevCategory.Name = Category.Name;
            return true;
        }

        public async Task<bool> UpdateCategory()
        {
            await _apiService.UpdateNoteCategoryAsync(Category);
            _prevCategory.Name = Category.Name;
            return true;
        }

        public async Task<bool> DeleteCategory()
        {
            await _apiService.DeleteNoteCategoryAsync(Category);
            Category = new Category();
            _prevCategory.Name = null;
            return true;
        }

        public async void CancelChanges()
        {
            CopyVersion();
        }

        private void CopyVersion()
        {
            Category = new Category
            {
                Id = _prevCategory.Id,
                Name = _prevCategory.Name,
            };
        }
    }
}
